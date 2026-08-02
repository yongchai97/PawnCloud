import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { PawnTicketsRoutingModule } from './pawn-tickets-routing.module';
import { PawnTicketsComponent } from './pawn-tickets.component';
import { CreatePawnTicketDialogComponent } from './create-pawn-ticket/create-pawn-ticket-dialog.component';
import { EditPawnTicketDialogComponent } from './edit-pawn-ticket/edit-pawn-ticket-dialog.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        SharedModule,
        PawnTicketsRoutingModule,
        CommonModule,
        PawnTicketsComponent,
        CreatePawnTicketDialogComponent,
        EditPawnTicketDialogComponent,
    ],
})
export class PawnTicketsModule {}