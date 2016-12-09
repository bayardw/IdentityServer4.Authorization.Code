# IdentityServer4.Authorization.Code
An IdentityServer4 Example of an OAuth 2.0 Authorization Code Flow (Grant). This project grew out of the need for an example IdentityServer4 client using the OAuth 2.0 Authorization Code Flow. This example doesn't use OpenId Connect (OIDC).

## Getting Started
### Prerequisites
This example requires:
- Visual Studio 2015
- .Net Core CLI and Visual Studio tooling for .Net Core (Preview 1 or later). The installer can be found here: https://www.microsoft.com/net/core#windows

The solution consists of two projects:
- An Identity Provier (IdP) command line application built on IdentityServer4 and .Net Core.
- An MVC Client built using Owin

**Both projects must start for the sample to work** when you press F5. In the Solution Explorer, right-click on the Solution at the top, select Set Startup Projects from the context menu. Pick Multiple Projects and set the action of both to Start. Pressing F5 now will start the identity provider and client.

Once the MVC project is running, select Sign-in from the menu bar. Here are some working credentials:
- user: user
- password: pass123

If the authentication with the IdP goes well, the client home/index page will enumerate the claims passed in on the Access Token.

## Technology Stack Choices
IdentityServer4 was chosen because getting logging output and therefore debugging was so easy; problems show up right away in the command line window. 

The choice of Owin (.Net Framework, pre-Core) for the client was purely for expediency. The project that drove the initial coding required calling into some legacy code that proved harder in .Net Core. 

## IdentityServer4
Verifing that a user is who they claim to be is the primary role of an Identity Provider (IdP). A robust provider like IdentityServer4 also:
- Provides screens for user to enter their username and password
- Reads the user database / datastore
- Keeps the user logged into the IdP itself using Cookie Middleware (configurable) so that subsequent calls don't show the login screen until the session expires

In IdentityServer3, the views, JavaScript, CSS, etc. to render those login / authorization screens were embedded resources in an assembly. In IdentityServer4, those files must be added. This makes customization tremendously easy, but does require that initial download. Those files are already included in this project, but for your own projects, visit https://github.com/IdentityServer/IdentityServer4.Quickstart.UI.

##Further Reading
- Cookie Middleware - https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie
- IdentityServer4 Getting Started - https://www.scottbrady91.com/Identity-Server/Getting-Started-with-IdentityServer-4
- Call for samples - http://stackoverflow.com/questions/37309986/identity-server-4-authorization-code-flow-example

##License
This project is licensed under the MIT License - see the license.md file for details.

## Acknowledgments
This code was inspired by Scott Brady's blog postings on the Implcit Flow and IdentityServer 3:  https://www.scottbrady91.com/Identity-Server/Identity-Server-3-Standalone-Implementation-Part-1

