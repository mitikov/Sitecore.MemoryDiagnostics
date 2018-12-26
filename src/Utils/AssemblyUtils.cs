namespace Sitecore.MemoryDiagnostics.Utils
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Threading.Tasks;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.StringExtensions;

  /// <summary>
  /// Handy class to operate with <see cref="Assembly"/> instances.
  /// <para>See <see cref="EnsureBinAssembliesLoaded"/> as a sample.</para>
  /// </summary>
  public static class AssemblyUtils
  {
    private static bool allAssembliesLoadedFromBin;

    public static IEnumerable<Type> ArgTypesFromGenericTpDiffOverload(Type genericInterface, Type attributeToCheck)
    {
      Assert.ArgumentNotNull(genericInterface, "genericInterface");
      Assert.IsTrue(genericInterface.IsGenericType, "{0} not generic".FormatWith(genericInterface.FullName));
      var genericArgumentType = genericInterface.GetGenericArguments().FirstOrDefault();

      Assert.IsNotNull(genericArgumentType, "{0} does not have generic arguments".FormatWith(genericInterface));

      var genericDefinition = genericInterface.GetGenericTypeDefinition();
      Assert.IsNotNull(genericDefinition, "no generic definition for {0}".FormatWith(genericInterface));
      var typesOfGenericInterface = ImplementInterfaceTypes(genericInterface, attributeToCheck);

      return (from tpImplementsTgIntf in typesOfGenericInterface
        let allInterfaces = tpImplementsTgIntf.GetInterfaces()
        from intf in allInterfaces
        where intf != genericInterface
        where intf.IsGenericType
        where intf.GetGenericTypeDefinition() == genericDefinition
        let interfaceArguments = intf.GenericTypeArguments
        from equatableType in interfaceArguments
        where genericArgumentType.IsAssignableFrom(equatableType)
        from interfaceArgument in interfaceArguments
        select interfaceArgument).Distinct();
    }


    /// <summary>
    /// Ensures an attempt to load all assemblies from bin folder was done.
    /// <para>This can be needed to find all class instances that implement given interface, and allow dynamic loading for application extensions.</para>
    /// </summary>
    public static void EnsureBinAssembliesLoaded()
    {
      if (allAssembliesLoadedFromBin)
        return;
      LoadAllBinDirectoryAssemblies();
      allAssembliesLoadedFromBin = true;
    }

    /// <summary>
    /// Provides non-abstract types that implement given interface.
    /// <para>Types are checked from assemlies that are loaded in <see cref="AppDomain.CurrentDomain"/> and decorated with <paramref name="attributeOnAssembly"/>.</para>
    /// </summary>
    /// <param name="interfaceType">Type of the interface.</param>  
    /// <param name="attributeOnAssembly">An attribute that an assembly must be decorated with to be eligible for search.</param>
    /// <returns></returns>
    public static Type[] ImplementInterfaceTypes([NotNull] Type interfaceType, Type attributeOnAssembly)
    {
      Assert.ArgumentNotNull(interfaceType, "interfaceType");
      Assert.IsTrue(interfaceType.IsInterface, "{0} is not interface".FormatWith(interfaceType.FullName));

      // Cudos to http://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
      var types = new List<Type>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(ass => ass.IsDefined(attributeOnAssembly)))
      {
        try
        {
          types.AddRange(assembly.GetExportedTypes().Where(t => !t.IsInterface).Where(t => !t.IsAbstract).Where(interfaceType.IsAssignableFrom));
        }
        catch (Exception ex)
        {
          Logger.Error(ex, "Failed to extract exported types from runtime.");
        }
      }

      return types.ToArray();
    }

    /// <summary>
    /// Loads all bin directory assemblies.
    /// <para>Consider using <see cref="EnsureBinAssembliesLoaded"/> to ensure single attempt to load assemblies.</para>
    /// </summary>
    public static void LoadAllBinDirectoryAssemblies()
    {
      Trace.WriteLine("Directory: " + System.AppDomain.CurrentDomain.BaseDirectory);
      Parallel.ForEach(Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories), dll =>
      {
        try
        {
          Assembly loadedAssembly = Assembly.LoadFile(dll);
        }
        catch (FileLoadException loadEx)
        {
        }
 // The Assembly has already been loaded.
        catch (BadImageFormatException imgEx)
        {
        }
 // If a BadImageFormatException exception is thrown, the file is not an assembly.
        catch (Exception ex)
        {
          Logger.SingleError(ex);
        }
      }); // foreach dll
    }

    public static IEnumerable<Type> TypesFromGenericTpDiffOverload(Type genericInterface, Type attributeToCheck)
    {
      Assert.ArgumentNotNull(genericInterface, "genericInterface");
      Assert.IsTrue(genericInterface.IsGenericType, "{0} not generic".FormatWith(genericInterface.FullName));

      var genericDefinition = genericInterface.GetGenericTypeDefinition();
      Assert.IsNotNull(genericDefinition, "no generic definition for {0}".FormatWith(genericInterface));

      var typesOfGenericInterface = ImplementInterfaceTypes(genericInterface, attributeToCheck);

      return (from candidateType in typesOfGenericInterface
        let interfaces = candidateType.GetInterfaces()
        from intf in interfaces
        where intf != genericInterface // loopedOne
        where intf.IsGenericType
        where intf.GetGenericTypeDefinition() == genericDefinition
        select candidateType).Distinct();
    }
  }
}