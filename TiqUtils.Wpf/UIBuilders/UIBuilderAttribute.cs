using System;

namespace TiqUtils.Wpf.UIBuilders
{
    // ReSharper disable once InconsistentNaming
    public sealed class UIBuilderAttribute : Attribute
    {
        public string BuilderTypeName { get; }

        public UIBuilderAttribute(string builderTypeName)
        {
            BuilderTypeName = builderTypeName;
        }
    }
}