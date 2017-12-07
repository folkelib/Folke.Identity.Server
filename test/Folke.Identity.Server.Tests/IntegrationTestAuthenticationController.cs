using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Folke.Elm;
using Folke.Elm.Sqlite;
using Folke.Identity.Elm;
using Folke.Identity.Server.Services;
using Folke.Identity.Server.Views;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Folke.Identity.Server.Tests
{
    public class IntegrationTestAuthenticationController
    {
        private readonly HttpClient client;
        private const string Administrator = "Admin";
        private const string AdminUserName = "admin@sample.com";
        private const string AdminPassword = "p@sSword18";


        public IntegrationTestAuthenticationController()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<SampleStartup>());
            client = server.CreateClient();
        }

        [Fact]
        public async Task Login()
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new { email = AdminUserName, password = AdminPassword }), Encoding.UTF8, "application/json");
            var responseFromLogin = await client.PutAsync("api/authentication/login", content);
            Assert.Equal(HttpStatusCode.OK, responseFromLogin.StatusCode);
        }

        [Fact]
        public async Task RegisterWithUsername()
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new { username = "TestUsername", email = "test@test.test", password = "p@sSword20", confirmPassword = "p@sSword20" }), Encoding.UTF8, "application/json");
            var responseFromRegister = await client.PostAsync("api/authentication/register", content);
            Assert.Equal(HttpStatusCode.OK, responseFromRegister.StatusCode);
        }

        [Fact]
        public async Task RegisterWithoutUsername()
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new { email = "test2@test.test", password = "p@sSword21", confirmPassword = "p@sSword21" }), Encoding.UTF8, "application/json");
            var responseFromRegister = await client.PostAsync("api/authentication/register", content);
            Assert.Equal(HttpStatusCode.OK, responseFromRegister.StatusCode);
        }

        public class SampleStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                var temp = Path.GetRandomFileName();
                services.AddElm<SqliteDriver>(options =>
                {
                    options.ConnectionString = $"Data Source={temp}.db";
                });
                services.AddIdentity<IdentityUser<int>, IdentityRole<int>>();
                services.AddMvc().AddIdentityServer<int, IdentityUser<int>, BaseUserView<int>, IdentityRole<int>, BaseRoleView<int>>();
                services.AddElmIdentity<IdentityUser<int>, IdentityRole<int>, int>();
                services.AddIdentityServer<IdentityUser<int>, int, EmailService, UserService, BaseUserView<int>>();
                services.AddRoleIdentityServer<IdentityRole<int>, RoleService, BaseRoleView<int>>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("UserList", policy =>
                    {
                        policy.RequireRole(Administrator);
                    });
                    options.AddPolicy("Role", policy =>
                    {
                        policy.RequireRole(Administrator);
                    });
                });
            }

            public class RoleService : IRoleService<IdentityRole<int>, BaseRoleView<int>>
            {
                public BaseRoleView<int> MapToRoleView(IdentityRole<int> role)
                {
                    return new BaseRoleView<int> { Id =  role.Id, Name = role.Name };
                }

                public IdentityRole<int> CreateNewRole(string name)
                {
                    return new IdentityRole<int> { Name = name };
                }
            }

            public class UserService : BaseUserService<IdentityUser<int>, BaseUserView<int>>
            {
                public UserService(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser<int>> userManager) : base(httpContextAccessor, userManager)
                {
                }

                public override BaseUserView<int> MapToUserView(IdentityUser<int> user)
                {
                    if (user == null) return new BaseUserView<int>();
                    return new BaseUserView<int> { Email = user.Email, Id = user.Id, EmailConfirmed = user.EmailConfirmed, HasPassword = !string.IsNullOrEmpty(user.PasswordHash), Logged = true, UserName = user.UserName };
                }

                public override IdentityUser<int> CreateNewUser(string userName, string email, bool emailConfirmed)
                {
                    return new IdentityUser<int> { UserName = userName, Email = email, EmailConfirmed = emailConfirmed };
                }

                public override Task<IList<IdentityUser<int>>> Search(UserSearchFilter name, int offset, int limit, string sortColumn)
                {
                    throw new NotImplementedException();
                }
            }

            public void Configure(IApplicationBuilder app, IFolkeConnection connection, IHostingEnvironment environment, RoleManager<IdentityRole<int>> roleManager, UserManager<IdentityUser<int>> userManager, ApplicationPartManager applicationPartManager)
            {
                app.UseAuthentication();
                app.UseMvc();
                app.UseRequestLocalization();

                using (var transaction = connection.BeginTransaction())
                {
                    connection.UpdateIdentityUserSchema<int, IdentityUser<int>>();
                    connection.UpdateIdentityRoleSchema<int, IdentityUser<int>, IdentityRole<int>>();
                    CreateAdministrator(roleManager, userManager).GetAwaiter().GetResult();
                    transaction.Commit();
                }
            }

            private static async Task CreateAdministrator(RoleManager<IdentityRole<int>> roleManager, UserManager<IdentityUser<int>> userManager)
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = Administrator });

                var result = await userManager.CreateAsync(new IdentityUser<int> { UserName = AdminUserName, Email = AdminUserName }, AdminPassword);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(AdminUserName);
                    await userManager.AddToRoleAsync(user, Administrator);
                }
            }

            public class EmailService : IUserEmailService<IdentityUser<int>>
            {
                public Task SendEmailConfirmationEmail(IdentityUser<int> user, string code)
                {
                    return Task.FromResult(0);
                }

                public Task SendPasswordResetEmail(IdentityUser<int> user, string code)
                {
                    return Task.FromResult(0);
                }
            }
        }
    }
}
