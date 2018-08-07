using Microsoft.VisualStudio.TestTools.UnitTesting;
using SA46Team1_Web_ADProj.Controllers;
using SA46Team1_Web_ADProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SA46Team1_Web_ADProj.Controllers.Tests
{
    [TestClass()]
    public class LoginTests
    {
        [TestMethod()]
        public void LoginTest()
        {
            //using (SSISdbEntities e = new SSISdbEntities())
            //{
            //    e.Configuration.ProxyCreationEnabled = false;
            //}

            var controller = new MainController();
            var nullResult = controller.Login();
            Assert.IsNotNull(nullResult);
        }

        [TestMethod()]
        public void LoginSuccessTest()
        {
            var controller = new MainController();
            UserModel user = new UserModel();
            user.Username = "johnwarton@lu.com";
            user.Password = "123";
            var loginResult = controller.Login(user);

            Assert.IsNotNull(loginResult);
            Assert.IsInstanceOfType(loginResult, typeof(Task<ActionResult>));

        }

        [TestMethod()]
        public void LoginFailTest()
        {
            var mController = new MainController();
            UserModel fakeUser = new UserModel();
            fakeUser.Username = "abc";
            fakeUser.Password = "123";

            var failLogin = mController.Login(fakeUser);
            var viewresult = failLogin.Result as ViewResult;
            Assert.AreEqual("Login", viewresult.ViewName);
        }
    }
}