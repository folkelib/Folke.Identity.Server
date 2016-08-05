Folke.Identity.server
======

This library adds MVC 6 Web API that allows to authenticate with a Single Page Application using Microsoft.AspNet.Identity.

[![Build status](https://ci.appveyor.com/api/projects/status/u6d3kw7rm97yk62g?svg=true)](https://ci.appveyor.com/project/acastaner/folke-identity-server)

## Usage

Modify your `ConfigureServices(IServiceCollection services)` method to add: 
```cs
services.AddIdentityServer<TUser, TKey, TUserEmailService, TUserService, TUserView>();
```
where `TUser` is your user class type, `TKey` its primary key type, `TUserEmailService`
an implementation of `IUserEmailService`,  `TUserService` an implementation of
`IUserService`, and TUserView a view on `TUser` that will be returned by the controllers.

The `IUserEmailService` service is used to send confirmation e-mails and to retrieve
lost passwords (for now, phone numbers are not supported for that).

You can then inherit from `BaseAuthenticationController` and `BaseUserController`
to create controllers that allow registration and login from an SPA using Web API.
