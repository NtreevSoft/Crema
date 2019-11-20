using Ntreev.Crema.Services;

namespace Ntreev.CremaServer.Tests.Extensions
{
    public static class ITypeTemplateExtensions
    {
        public static ITypeTemplate CreateAddNew(this ITypeTemplate template, Authentication authentication,
            string typeName, long value, int? index = null)
        {
            var type = template.AddNew(authentication);

            type.SetName(authentication, typeName);

            type.SetValue(authentication, value);

            if (index != null)
            {
                type.SetIndex(authentication, index.Value);
            }

            template.EndNew(authentication, type);
            return template;
        }
    }
}
