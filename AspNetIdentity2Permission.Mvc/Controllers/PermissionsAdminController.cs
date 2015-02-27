﻿using AspNetIdentity2Permission.Mvc.Models;
using AutoMapper;
using Infragistics.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace AspNetIdentity2Permission.Mvc.Controllers
{
    public class PermissionsAdminController : BaseController
    {
        [Description("权限列表")]
        public async Task<ActionResult> Index()
        {
            //var roleViews = await GetRoleViews();
            //ViewBag.RoleID = new SelectList(roleViews, "ID", "Name", roleViews.FirstOrDefault().Id);
            var permissions = await _db.Permissions.ToListAsync();
            //创建ViewModel
            var permissionViews = new List<PermissionViewModel>();

            var map = Mapper.CreateMap<ApplicationPermission, PermissionViewModel>();
            permissions.Each(t =>
            {
                var view = Mapper.Map<PermissionViewModel>(t);

                permissionViews.Add(view);
            });
            //排序
            permissionViews.Sort(new PermissionViewModelComparer());
            return View(permissionViews);
        }

        // GET: PermissionsAdmin/Details/5
        [Description("权限详情")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationPermission applicationPermission = _db.Permissions.Find(id);
            if (applicationPermission == null)
            {
                return HttpNotFound();
            }
            var view = new PermissionViewModel
            {
                Id = applicationPermission.Id,
                Controller = applicationPermission.Controller,
                Action = applicationPermission.Action,
                Description = applicationPermission.Description
            };
            return View(view);
        }

        // GET: PermissionsAdmin/Create
        [Description("新建权限，列表")]
        [GridDataSourceAction]
        public ActionResult Create()
        {
            //创建ViewModel
            var permissionViews = new List<PermissionViewModel>();
            //取程序集中权限
            var allPermissions = _permissionsOfAssembly;
            //取数据库已有权限
            var dbPermissions = _db.Permissions.ToList();
            //取两者差集
            var permissions = allPermissions.Except(dbPermissions, new ApplicationPermissionEqualityComparer());
            var map = Mapper.CreateMap<ApplicationPermission, PermissionViewModel>();
            permissions.Each(t =>
            {
                var view = Mapper.Map<PermissionViewModel>(t);

                permissionViews.Add(view);
            });
            //排序
            permissionViews.Sort(new PermissionViewModelComparer());

            return View(permissionViews.AsQueryable());
        }

        [Description("新建权限，保存")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IEnumerable<PermissionViewModel> data)
        {
            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "参数不能为空");
            }
            foreach (var item in data)
            {
                //创建权限
                var permission = new ApplicationPermission
                {
                    Id = item.Id,
                    Action = item.Action,
                    Controller = item.Controller,
                    Description = item.Description
                };
                _db.Permissions.Add(permission);
            }
            //保存
            await _db.SaveChangesAsync();

            //方法2，使用Newtonsoft.Json序列化结果对象
            //格式为json字符串，客户端需要解析，即反序列化
            var result = JsonConvert.SerializeObject(new { Success = true });
            return new JsonResult { Data = result };
        }

        // GET: PermissionsAdmin/Edit/5
        [Description("编辑权限")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationPermission applicationPermission = _db.Permissions.Find(id);
            if (applicationPermission == null)
            {
                return HttpNotFound();
            }
            var view = new PermissionViewModel
            {
                Id = applicationPermission.Id,
                Action = applicationPermission.Action,
                Controller = applicationPermission.Controller,
                Description = applicationPermission.Description
            };
            return View(view);
        }

        // POST: PermissionsAdmin/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [Description("编辑权限，保存")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Controller,Action,Description")] PermissionViewModel view)
        {
            if (ModelState.IsValid)
            {
                var model = new ApplicationPermission
                {
                    Id = view.Id,
                    Action = view.Action,
                    Controller = view.Controller,
                    Description = view.Description
                };
                _db.Entry(model).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(view);
        }

        // GET: PermissionsAdmin/Delete/5
        [Description("删除权限")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationPermission applicationPermission = _db.Permissions.Find(id);
            if (applicationPermission == null)
            {
                return HttpNotFound();
            }
            var view = new PermissionViewModel
            {
                Id = applicationPermission.Id,
                Action = applicationPermission.Action,
                Controller = applicationPermission.Controller,
                Description = applicationPermission.Description
            };
            return View(view);
        }

        // POST: PermissionsAdmin/Delete/5
        [Description("删除权限，保存")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationPermission applicationPermission = _db.Permissions.Find(id);
            _db.Permissions.Remove(applicationPermission);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
