# OpenIDServiceAPI

Configuration Instructions

ClientID, Client Authority, and Scope Setup:
Update the ClientID, Authority, and Scope values in your App.config file according to your Azure AD app registration.

Scope: Expose an API with access_as_user permission and include it in your scope. Example:
api://localhost/{clientid}/access_as_user
(The application URI will vary depending on your Azure configuration.)

Token Cache Path:
Specify a local path where your authentication token will be cached using a file-based cache mechanism. This cached token will allow silent token acquisition for subsequent logins.

Service API URL:
Provide the URL for your service API.

![image](https://github.com/user-attachments/assets/30363a0e-6daf-484d-9c7e-2b2ddee8dd18)

The acquired access_token will be passed to the TrimClient library as an authorization header. The service API will then use the C# libraries to run with OpenID Connect authentication.







