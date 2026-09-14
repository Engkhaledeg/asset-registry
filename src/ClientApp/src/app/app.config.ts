import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { APP_INITIALIZER, ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import {
  MSAL_GUARD_CONFIG,
  MSAL_INSTANCE,
  MSAL_INTERCEPTOR_CONFIG,
  MsalBroadcastService,
  MsalGuard,
  MsalInterceptor,
  MsalService
} from '@azure/msal-angular';
import { routes } from './app.routes';
import {
  createMsalGuardConfiguration,
  createMsalInstance,
  createMsalInterceptorConfiguration
} from './core/auth/msal.config';

function initialiseMsal(msal: MsalService): () => Promise<void> {
  return () => msal.instance.initialize();
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: MSAL_INSTANCE, useFactory: createMsalInstance },
    { provide: MSAL_GUARD_CONFIG, useFactory: createMsalGuardConfiguration },
    { provide: MSAL_INTERCEPTOR_CONFIG, useFactory: createMsalInterceptorConfiguration },
    { provide: HTTP_INTERCEPTORS, useClass: MsalInterceptor, multi: true },
    { provide: APP_INITIALIZER, useFactory: initialiseMsal, deps: [MsalService], multi: true },
    MsalService,
    MsalGuard,
    MsalBroadcastService
  ]
};
