using System;

namespace TiqUtils.Wpf.UIBuilders
{
    public class PropertyGroupAttribute : Attribute
    {
        public string GroupName { get; }

        public PropertyGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}