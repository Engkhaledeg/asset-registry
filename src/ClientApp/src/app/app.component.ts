import { AsyncPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth/auth.service';

@Component({
  selector: 'ar-root',
  standalone: true,
  imports: [AsyncPipe, RouterLink, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  readonly signedInUser$ = this.auth.signedInUser$;

  constructor(private readonly auth: AuthService) {}

  ngOnInit(): void {
    this.auth.startTrackingAccount();
  }

  signIn(): void {
    this.auth.signIn();
  }

  signOut(): void {
    this.auth.signOut();
  }
}
