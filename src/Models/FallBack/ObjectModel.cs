namespace Sitecore.MemoryDiagnostics.Models.FallBack
{
  using System;
  using Attributes;
  using BaseMappingModel;
  using ModelMetadataInterfaces;

  [ModelMapping(typeof(object))]
  public class ObjectModel : ClrObjectMappingModel, IEquatable<ObjectModel>
  {
    public override string Caption => $"{base.Caption} + {this.ToString()}";

    public bool Equals(ObjectModel other)
    {
      return this.Address == other?.Address;
    }

    public override string ToString()
    {
      return "Object " + this.HexAddress;
    }
  }
}