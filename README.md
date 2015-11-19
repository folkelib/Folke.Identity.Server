Folke.Identity.server
======

This library adds MVC 6 Web API that allows to authenticate with a Single Page Application using Microsoft.AspNet.Identity.

## Usage

Modify your `ConfigureServices(IServiceCollection services)` method to add: 
```cs
services.AddIdentityServer<TUser, TKey, TUserEmailService>();
```
where `TUser` is your user class type, `TKey` its primary key type, and TUserEmailService
an implementation of `IUserEmailService`.

The `IUserEmailService` service is used to send confirmation e-mails and to retrieve
lost passwords (for now, phone numbers are not supported for that).

You can then inherit from `BaseAuthenticationController` and `BaseUserController`
to create controllers that allow registration and login from an SPA using Web API.
