import { Directive, Input, ViewContainerRef, TemplateRef, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Directive({
  selector: '[appHasRole]' // *appHasRole * = <ng-template>
})
export class HasRoleDirective implements OnInit, OnDestroy {
  @Input() appHasRole: string [];
  stop$ = new Subject();
  isVisible = false;
  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, private authService: AuthService) { }
  ngOnInit() {
    this.authService.refreshedRole.asObservable().pipe(
      takeUntil(this.stop$)
    ).subscribe(() => {
      const userRoles = this.authService.decodedToken.role as Array<string>;
      if (!userRoles) {
        this.viewContainerRef.clear();
      }
      if (this.authService.roleMatch(this.appHasRole)) {
          if (!this.isVisible) {
            this.isVisible = true;
            this.viewContainerRef.createEmbeddedView(this.templateRef);
          }
      } else {
            this.isVisible = false ;
            this.viewContainerRef.clear();
      }
    });
  }
  ngOnDestroy() {
    this.stop$.next();
  }
}
