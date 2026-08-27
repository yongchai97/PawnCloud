import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { PawnTicketsRoutingModule } from './pawn-tickets-routing.module';
import { PawnTicketsComponent } from './pawn-tickets.component';

@NgModule({
    imports: [CommonModule, FormsModule, SharedModule, PawnTicketsRoutingModule, PawnTicketsComponent],
})
export class PawnTicketsModule {}
