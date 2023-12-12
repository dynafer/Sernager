using Sernager.Runner.Managers;
using Sernager.Runner.Options;

namespace Sernager.Runner.Builders;

/// <include file='docs/builders/setting_builder.xml' path='Class/Description'/> 
public interface ISettingBuilder
{
    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="AddSettingName"]'/>
    ISettingBuilder AddSettingName(string name, EAddDataOption option = EAddDataOption.SkipIfExists);
    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="AddSettingNames"][@Type="WithDefaultOption"]'/>
    ISettingBuilder AddSettingNames(params string[] names);
    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="AddSettingNames"][@Type="WithGivenOption"]'/>
    ISettingBuilder AddSettingNames(IEnumerable<string> names, EAddDataOption option = EAddDataOption.SkipIfExists);
    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="RemoveSettingName"]'/>
    ISettingBuilder RemoveSettingName(string name);
    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="RemoveSettingNames"]'/>
    ISettingBuilder RemoveSettingNames(params string[] names);
    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="UseManager"]'/>
    ISettingManager UseManager(string name);
}
