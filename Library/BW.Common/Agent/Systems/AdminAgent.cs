using BW.Common.Caching;
using BW.Common.Entities.Systems;
using BW.Common.Models.Enums;
using BW.Common.Models.Systems;
using SP.StudioCore.Security;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Agent.Systems
{
    /// <summary>
    /// 系统管理员
    /// </summary>
    public sealed class AdminAgent : AgentBase<AdminAgent>
    {
        /// <summary>
        /// 系统管理员登录
        /// </summary>
        /// <param name="adminName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(string adminName, string password, out Guid session)
        {
            session = Guid.Empty;
            SystemAdmin admin = this.ReadDB.ReadInfo<SystemAdmin>(t => t.AdminName == adminName);
            if (admin == null)
            {
                if (_createDefaultAdmin())
                {
                    return this.FaildMessage("已创建默认管理员账号");
                }
                return this.FaildMessage("管理员不存在");
            }
            if (admin.Status != AdminStatus.Normal) return this.FaildMessage("已被锁定");
            if (admin.Password != Encryption.toMD5(password)) return this.FaildMessage("密码错误");
            admin.LoginIP = IPAgent.IP;
            admin.LoginAt = WebAgent.GetTimestamps();
            this.WriteDB.Update(admin, t => t.LoginIP, t => t.LoginAt);
            session = SystemCaching.Instance().SaveToken(admin.ID);
            return true;
        }

        public SystemAdmin GetAdminInfo(int adminId)
        {
            return this.ReadDB.ReadInfo<SystemAdmin>(t => t.ID == adminId);
        }

        public SystemAdminModel GetAdminModel(Guid token)
        {
            int adminId = this.GetAdminID(token);
            if (adminId == 0) return default;

            SystemAdminModel admin = SystemCaching.Instance().GetAdminInfo(adminId);
            if (!admin)
            {
                admin = this.GetAdminInfo(adminId);
                if (!admin) return default;
                SystemCaching.Instance().SaveAdminInfo(admin);
            }
            return admin;
        }

        /// <summary>
        /// 获取当前登录的管理员账号
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public int GetAdminID(Guid token)
            => SystemCaching.Instance().GetAdminID(token);


        /// <summary>
        /// 创建默认管理员账号
        /// </summary>
        private bool _createDefaultAdmin()
        {
            if (this.ReadDB.Exists<SystemAdmin>()) return false;
            return this.WriteDB.Insert(new SystemAdmin
            {
                AdminName = "admin",
                Password = Encryption.toMD5("admin"),
                Status = AdminStatus.Normal
            });
        }
    }
}
