using System;
using System.ComponentModel.DataAnnotations;

namespace Mbc.Log4Tc.Service.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class TypeClassValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            string clazz = value.ToString();
            var type = Type.GetType(clazz, false);

            return type != null;
        }
    }
}
