import { Injectable } from '@angular/core';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { AccountInfo, InteractionStatus } from '@azure/msal-browser';
import { BehaviorSubject, Observable, filter, map } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly account = new BehaviorSubject<AccountInfo | null>(null);

  constructor(
    private readonly msal: MsalService,
    private readonly broadcast: MsalBroadcastService
  ) {}

  get signedInUser$(): Observable<string | null> {
    return this.account.pipe(map((account) => account?.username ?? null));
  }

  get isSignedIn$(): Observable<boolean> {
    return this.account.pipe(map((account) => account !== null));
  }

  startTrackingAccount(): void {
    this.msal.handleRedirectObservable().subscribe(() => this.refreshActiveAccount());

    this.broadcast.inProgress$
      .pipe(filter((status) => status === InteractionStatus.None))
      .subscribe(() => this.refreshActiveAccount());
  }

  signIn(): void {
    this.msal.loginRedirect({ scopes: [environment.entraId.apiScope] });
  }

  signOut(): void {
    this.msal.logoutRedirect();
  }

  private refreshActiveAccount(): void {
    const accounts = this.msal.instance.getAllAccounts();
    const activeAccount = this.msal.instance.getActiveAccount() ?? accounts[0] ?? null;

    if (activeAccount !== null) {
      this.msal.instance.setActiveAccount(activeAccount);
    }

    this.account.next(activeAccount);
  }
}
