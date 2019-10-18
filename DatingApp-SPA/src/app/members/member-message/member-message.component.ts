import { Component, OnInit, Input } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Message } from 'src/app/_models/Message';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-message',
  templateUrl: './member-message.component.html',
  styleUrls: ['./member-message.component.css']
})
export class MemberMessageComponent implements OnInit {
  @Input() recipientId;
  messages: Message[];
  newMessage: any = {};
  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }
  loadMessages() {
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
    .pipe(
      tap(messages => {
        // tslint:disable-next-line:prefer-for-of
        for (let i = 0; i < messages.length ; i++) {
          // tslint:disable-next-line:triple-equals
          if (messages[i].isRead == false && messages[i].recipientId == this.authService.decodedToken.nameid) {
            this.userService.markAsRead(messages[i].id, this.authService.decodedToken.nameid);
          }
        }
      })
    )
    .subscribe((res) => {
      this.messages = res;
    }, error => this.alertify.error(error.error));
  }
  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
                .subscribe((res: Message) => {
                  this.messages.unshift(res);
                  this.newMessage.content = '';
                }, error => {
                  this.alertify.error(error.error);
                });
  }

}
