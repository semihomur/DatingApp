import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule, ModalModule } from 'ngx-bootstrap';
import { TimeAgoPipe} from 'time-ago-pipe';
import { NgxGalleryModule} from 'ngx-gallery';
import { AppComponent } from './app.component';
import { HeaderComponent } from './_components/header/header.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './_components/home/home.component';
import { RegisterComponent } from './_components/register/register.component';
import { AlertifyService } from './_services/alertify.service';
import { MemberListComponent } from './_components/members/member-list/member-list.component';
import { ListsComponent } from './_components/lists/lists.component';
import { MessagesComponent } from './_components/messages/messages.component';
import { RouterModule } from '@angular/router';
import { AppRoutes } from './app-routes';
import { AuthGuard } from './_guard/auth.guard';
import { UserService } from './_services/user.service';
import { MemberCardComponent } from './_components/members/member-card/member-card.component';
import { MemberDetailComponent } from './_components/members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-listresolver';
import { MemberEditComponent } from './_components/members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guard/prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './_components/members/photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import { ListsResolver } from './_resolvers/lists.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver';
import { MemberMessageComponent } from './_components/members/member-message/member-message.component';
import { AdminPanelComponent } from './_components/admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/hasRole.directive';
import { UserManagementComponent } from './_components/admin/user-management/user-management.component';
import { PhotoManagementComponent } from './_components/admin/photo-management/photo-management.component';
import { AdminService } from './_services/admin.service';
import { RolesModalComponent } from './_components/admin/roles-modal/roles-modal.component';
import { FilterStringPipe } from './_pipe/filterString.pipe';
import { AuthInterceptor } from './_guard/auth.interceptor';
import { FooterComponent } from './_components/footer/footer.component';
import { DisableButtonDirective } from './_directives/disable-button.directive';
import { ReportComponent } from './_components/admin/report/report.component';
import { AdminStatisticsComponent } from './_components/admin/admin-statistics/admin-statistics.component';
import { DonutChartComponent } from './_components/admin/admin-statistics/donut-chart/donut-chart.component';
import {ChartsModule} from 'ng2-charts';

export function tokenGetter() {
   return localStorage.getItem('token');
}
@NgModule({
   declarations: [
      AppComponent,
      HeaderComponent,
      HomeComponent,
      RegisterComponent,
      MemberListComponent,
      ListsComponent,
      MessagesComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      PhotoEditorComponent,
      TimeAgoPipe,
      MemberMessageComponent,
      AdminPanelComponent,
      HasRoleDirective,
      UserManagementComponent,
      PhotoManagementComponent,
      RolesModalComponent,
      FilterStringPipe,
      FooterComponent,
      DisableButtonDirective,
      ReportComponent,
      AdminStatisticsComponent,
      DonutChartComponent
   ],
   imports: [
      BrowserModule,
      BrowserAnimationsModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      TabsModule.forRoot(),
      ModalModule.forRoot(),
      BsDropdownModule.forRoot(),
      BsDatepickerModule.forRoot(),
      RouterModule.forRoot(AppRoutes),
      PaginationModule.forRoot(),
      ButtonsModule.forRoot(),
      ModalModule.forRoot(),
      NgxGalleryModule,
      FileUploadModule,
      ChartsModule
   ],
   providers: [
      AuthService,
      {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true},
      AlertifyService,
      AuthGuard,
      UserService,
      MemberDetailResolver,
      MemberListResolver,
      MemberEditResolver,
      PreventUnsavedChanges,
      ListsResolver,
      MessagesResolver,
      AdminService
   ],
   entryComponents: [
      RolesModalComponent
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
