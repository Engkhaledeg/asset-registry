export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7043/api',
  entraId: {
    clientId: '<spa-app-registration-client-id>',
    tenantId: '<your-tenant-id>',
    redirectUri: 'http://localhost:4200',
    apiScope: 'api://<api-app-registration-client-id>/access_as_user'
  }
};
