import { Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { AssetFormComponent } from './features/assets/asset-form/asset-form.component';
import { AssetListComponent } from './features/assets/asset-list/asset-list.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'assets' },
  { path: 'assets', component: AssetListComponent, canActivate: [MsalGuard] },
  { path: 'assets/new', component: AssetFormComponent, canActivate: [MsalGuard] },
  { path: 'assets/:id', component: AssetFormComponent, canActivate: [MsalGuard] },
  { path: '**', redirectTo: 'assets' }
];
