# is4-authorization-code-flow
IdentityServer 4 Example of an Authorization Code Flow (Grant)

This project grew out of the need for an example IdentityServer 4 client using the Authorization Code Flow. This code was inspired by Scott Brady's blog postings on the Implcit Flow and IdentityServer 3:  https://www.scottbrady91.com/Identity-Server/Identity-Server-3-Standalone-Implementation-Part-1

The solution consists of two projects:
An Identity Provier (IdP) command line applicaton built on IdentityServer 4 and Dotnet Core.
An MVC Client built using Owin

Both projects must start for the sample to work. In the Solution Explorer, right-click on the Solution at the top, select Set Startup Projects from the context menu. Pick Multiple Projects and set the action of both to Start. Pressing F5 now will start the identity provider and client.

Once the MVC project is running, select Sign-in from the menu bar. Here are some working credentials:
user: user
password: pass123

The claims passed in on the Access Token will be enumerated. The client application, after validating the Access Token, will authenticate into the Cookie Middleware so the user is onsidered logged-in. 



