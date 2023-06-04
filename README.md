# Simple token authentication in .NET 7 API with OpenIddict

This is a very simple example of the password credentials flow.

Please bear in mind that this flow is not recommended by the OAuth 2.0 specification as it is the only grant type where the user password is directly exposed to the client application, which breaks the principle of least privilege and makes it unsuitable for third-party client applications that cannot be fully trusted by the authorization server.

Additional info can be found here https://oauth.net/2/grant-types/password/. 