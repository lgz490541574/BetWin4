//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BW.Common.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BW.Common.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;root ID=&quot;d447625878a246588a6805a5c1e70a98&quot;&gt;
        ///  &lt;memu name=&quot;商户会员&quot; icon=&quot;am-icon-users&quot; ID=&quot;ebf83955a2464aef9fb6f5c3158ab78f&quot;&gt;
        ///    &lt;menu name=&quot;商户管理&quot; href=&quot;merchant/site&quot; ID=&quot;67aa4d974305414d879233c817a6b735&quot; /&gt;
        ///    &lt;menu name=&quot;会员管理&quot; href=&quot;merchant/user&quot; ID=&quot;0e572d57836944f9a3a641e0b16e96c8&quot; /&gt;
        ///  &lt;/memu&gt;
        ///  &lt;memu name=&quot;报表管理&quot; icon=&quot;am-icon-pie-chart&quot; ID=&quot;3f1f66b27f6846ef9e4b2413786c47cf&quot;&gt;
        ///    &lt;menu name=&quot;财务报表&quot; href=&quot;report/settlement&quot; ID=&quot;72a40410172e400dab36a06286128a40&quot; /&gt;
        ///    &lt;menu name=&quot;数据统计&quot; href=&quot;report/st [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string SystemPermission {
            get {
                return ResourceManager.GetString("SystemPermission", resourceCulture);
            }
        }
    }
}
