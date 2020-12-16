import { Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {MessageData, MessageType} from '../model/models';

@Component({
  selector: 'app-dialog',
  templateUrl: './dialog.component.html',
  styleUrls: ['./dialog.component.scss']
})
export class DialogComponent {
  messageType = MessageType;
  constructor(@Inject(MAT_DIALOG_DATA) public data: MessageData) {}


}
