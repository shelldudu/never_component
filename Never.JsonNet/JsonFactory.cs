using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.JsonNet
{
    class JsonFactory
    {
    }

    //public class CnblogsJsonValueProviderFactory : ValueProviderFactory
    //{ public override IValueProvider GetValueProvider(ControllerContext controllerContext) { if (controllerContext == null) throw new ArgumentNullException("controllerContext"); if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)) { return null; } var bodyText = string.Empty; using (var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream)) { bodyText = reader.ReadToEnd(); } if (string.IsNullOrEmpty(bodyText)) return null; return new JObjectValueProvider(bodyText.StartsWith("[") ? JArray.Parse(bodyText) as JContainer : JObject.Parse(bodyText) as JContainer); } }
    //public class JObjectValueProvider : IValueProvider { private JContainer _jcontainer; public JObjectValueProvider(JContainer jcontainer) { _jcontainer = jcontainer; } public bool ContainsPrefix(string prefix) { return _jcontainer.SelectToken(prefix) != null; } public ValueProviderResult GetValue(string key) { var jtoken = _jcontainer.SelectToken(key); if (jtoken == null || jtoken.Type == JTokenType.Object) return null; return new ValueProviderResult(jtoken.ToObject<object>(), jtoken.ToString(), CultureInfo.CurrentCulture); } }
}
