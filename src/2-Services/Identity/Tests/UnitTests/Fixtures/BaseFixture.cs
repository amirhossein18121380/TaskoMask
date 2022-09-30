﻿using Microsoft.AspNetCore.Identity;
using NSubstitute;
using TaskoMask.BuildingBlocks.Test;
using TaskoMask.Services.Identity.Domain.Entities;
using TaskoMask.Services.Identity.UnitTests.Helpers;

namespace TaskoMask.Services.Identity.UnitTests.Fixtures
{

    /// <summary>
    /// 
    /// </summary>
    public class BaseFixture : UnitTestsBase
    {

        public TestUserManager TestUserManager;
        public TestSignInManager TestSignInManager;
        public List<User> TestUsers;
        public List<UserLogin> TestUserLogins;
        
        public BaseFixture()
        {

        }


        protected override void FixtureSetup()
        {
            TestUserManager = Substitute.For<TestUserManager>();
            TestSignInManager = Substitute.For<TestSignInManager>(TestUserManager);
            TestUsers = new List<User>();
            TestUserLogins= new List<UserLogin>();

            TestSignInManager.PasswordSignInAsync(userName: Arg.Any<string>(), password: Arg.Any<string>(), isPersistent: Arg.Any<bool>(), lockoutOnFailure: Arg.Any<bool>()).Returns(args =>
            {
                var userExist = TestUsers.Any(u => u.UserName == (string)args[0] && u.PasswordHash == (string)args[1]);

                return userExist ? new TestSignInResult(true) : new TestSignInResult(false);
            });


            TestUserManager.FindByNameAsync(userName: Arg.Any<string>()).Returns(args =>
            {
                return TestUsers.FirstOrDefault(u => u.UserName == (string)args[0]);
            });


            TestUserManager.CreateAsync(user: Arg.Any<User>(),password: Arg.Any<string>()).Returns(args =>
            {
                TestUsers.Add((User)args[0]);
                return new TestIdentityResult(true);
            });


            TestUserManager.AddLoginAsync(user: Arg.Any<User>(), login: Arg.Any<UserLoginInfo>()).Returns(args =>
            {
                var user = (User)args[0];
                var UserLoginInfo = (UserLoginInfo)args[1];
                var userlogin = new UserLogin
                {
                    UserId=user.Id,
                    LoginProvider= UserLoginInfo.LoginProvider,
                    ProviderDisplayName= UserLoginInfo.ProviderDisplayName,
                    ProviderKey= UserLoginInfo.ProviderKey,
                };
                TestUserLogins.Add(userlogin);
                return new TestIdentityResult(true);
            });
            
        }
    }
}