import { MsalGuardConfiguration, MsalInterceptorConfiguration } from '@azure/msal-angular';
import {
  BrowserCacheLocation,
  IPublicClientApplication,
  InteractionType,
  LogLevel,
  PublicClientApplication
} from '@azure/msal-browser';
import { environment } from '../../../environments/environment';

export function createMsalInstance(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: environment.entraId.clientId,
      authority: `https://login.microsoftonline.com/${environment.entraId.tenantId}`,
      redirectUri: environment.entraId.redirectUri,
      postLogoutRedirectUri: environment.entraId.redirectUri
    },
    cache: {
      cacheLocation: BrowserCacheLocation.SessionStorage
    },
    system: {
      loggerOptions: {
        logLevel: environment.production ? LogLevel.Error : LogLevel.Warning,
        piiLoggingEnabled: false
      }
    }
  });
}

export function createMsalGuardConfiguration(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Redirect,
    authRequest: { scopes: [environment.entraId.apiScope] }
  };
}

export function createMsalInterceptorConfiguration(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, string[] | null>([
    [environment.apiBaseUrl, [environment.entraId.apiScope]]
  ]);

  return {
    interactionType: InteractionType.Redirect,
    protectedResourceMap
  };
}
