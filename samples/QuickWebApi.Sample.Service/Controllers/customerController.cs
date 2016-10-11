﻿using QuickWebApi.Sample.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace QuickWebApi.Sample.Service.Controllers
{
    public class DB
    {
        static DB()
        {
            customers = new List<customer>();
            for (int i = 0; i < 10; i++)
            {
                customers.Add(new customer() { name = "name" + i, age = 30 + i, id = i, birthday = DateTime.Now.AddYears(-30).AddYears(-i), state = i / 2 == 0 });
            }
        }
        public static List<customer> customers;

    }


    [Route("api/customer_service/{action}/")]
    //[Authorize]
    public class customerController : ApiController, icustomer
    {
        [HttpGet]
        public IHttpActionResult list()
        {
            response_list res = new response_list();
            res.list = DB.customers.ToArray();
            res.count = res.list.Length;
            HttpContext.Current.Base().ApiModel();
            ws_model<string, response_list> model = HttpContext.Current.Base().ApiModel<string, response_list>(null);
            model.response = res;
            return Ok(model);
        }

        [HttpPost]
        public IHttpActionResult info(ws_model<int> model)
        {
            model.response = DB.customers.SingleOrDefault(c => c.id == model.request);
            return Ok(model);
        }

        [HttpPost]
        public IHttpActionResult update(ws_model<request_update, com_result> model)
        {
            var cust = DB.customers.SingleOrDefault(c => c.id == model.request.id);
            cust.name = model.request.name;
            model.response = new com_result();
            return Ok(model);
        }

        [HttpDelete]
        public IHttpActionResult del(ws_model<int, com_result> model)
        {
            DB.customers.RemoveAll(c => c.id == model.request);
            model.response = new com_result();
            return Ok(model);
        }

        [HttpPut]
        public IHttpActionResult save(ws_model<customer, com_result> model)
        {
            DB.customers.RemoveAll(c => c.id == model.request.id);
            DB.customers.Add(model.request);
            model.response = new com_result();
            return Ok(new result(model));
        }


        [HttpGet]
        public IHttpActionResult all(long timestamp)
        {
            //new webapi<icompany>().invoke(i => i.all, timestamp);
            return Ok(new result(DB.customers.Where(c => c.timestamp < timestamp)));
        }


        [HttpDelete]
        public IHttpActionResult delage(int age)
        {
            return Ok(new result(DB.customers.Where(c => c.age == age)));
        }


        [HttpGet]
        public IHttpActionResult pick(int id)
        {
            ws_model<object, customer> model = new ws_model<object, customer>();
            model.response = DB.customers.SingleOrDefault(c => c.id == id);
            return Ok(model);
        }


        [HttpGet]
        public IHttpActionResult query(int id, string name)
        {
            response_list res = new response_list();
            res.list = DB.customers.Where(c => c.id == id || c.name == name).ToArray();
            res.count = res.list.Length;
            ws_model<object, response_list> model = HttpContext.Current.Base().ApiModel<object, response_list>(res);
            return Ok(model);
        }

        [HttpPost]
        public IHttpActionResult newone(string name)
        {
            response_list res = new response_list();
            res.list = DB.customers.Where(c => c.name == name).ToArray();
            res.count = res.list.Length;
            return Ok(new result<response_list>(res));
        }

        [HttpPost]
        public IHttpActionResult trigger()
        {
            return Ok(HttpContext.Current.Base().ApiModel());
        }
    }
}
