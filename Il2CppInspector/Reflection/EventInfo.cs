﻿/*
    Copyright 2017 Katy Coe - http://www.hearthcode.org - http://www.djkaty.com

    All rights reserved.
*/

using System.Reflection;

namespace Il2CppInspector.Reflection
{
    public class EventInfo : MemberInfo
    {
        // IL2CPP-specific data
        public Il2CppEventDefinition Definition { get; }
        public int Index { get; }

        // Information/flags about the event
        public EventAttributes Attributes { get; }

        // Methods for the event
        public MethodInfo AddMethod { get; }
        public MethodInfo RemoveMethod { get; }
        public MethodInfo RaiseMethod { get; }

        // Event handler delegate type
        private Il2CppType eventType;
        public TypeInfo EventHandlerType => Assembly.Model.GetType(eventType, MemberTypes.TypeInfo);

        // True if the event has a special name
        public bool IsSpecialName => (Attributes & EventAttributes.SpecialName) == EventAttributes.SpecialName;

        public override MemberTypes MemberType => MemberTypes.Event;

        public EventInfo(Il2CppInspector pkg, int eventIndex, TypeInfo declaringType) :
            base(declaringType) {
            Definition = pkg.Metadata.Events[eventIndex];
            Index = eventIndex;
            Name = pkg.Strings[Definition.nameIndex];

            eventType = pkg.TypeUsages[Definition.typeIndex];
            if ((eventType.attrs & Il2CppConstants.FIELD_ATTRIBUTE_SPECIAL_NAME) == Il2CppConstants.FIELD_ATTRIBUTE_SPECIAL_NAME)
                Attributes |= EventAttributes.SpecialName;

            // NOTE: This relies on methods being added to TypeInfo.DeclaredMethods in the same order they are defined in the Il2Cpp metadata
            // add, remove and raise are method indices from the first method of the declaring type
            if (Definition.add >= 0)
                AddMethod = declaringType.DeclaredMethods[Definition.add];
            if (Definition.remove >= 0)
                RemoveMethod = declaringType.DeclaredMethods[Definition.remove];
            if (Definition.raise >= 0)
                RaiseMethod = declaringType.DeclaredMethods[Definition.raise];
        }
    }
}