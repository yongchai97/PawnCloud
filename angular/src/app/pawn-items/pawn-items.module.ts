import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { PawnItemsRoutingModule } from './pawn-items-routing.module';
import { PawnItemsComponent } from './pawn-items.component';
import { CreatePawnItemDialogComponent } from './create-pawn-item/create-pawn-item-dialog.component';
import { EditPawnItemDialogComponent } from './edit-pawn-item/edit-pawn-item-dialog.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        SharedModule,
        PawnItemsRoutingModule,
        CommonModule,
        PawnItemsComponent,
        CreatePawnItemDialogComponent,
        EditPawnItemDialogComponent,
    ],
})
export class PawnItemsModule {}
