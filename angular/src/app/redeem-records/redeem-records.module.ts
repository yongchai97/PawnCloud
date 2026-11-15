import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { RedeemRecordsRoutingModule } from './redeem-records-routing.module';
import { RedeemRecordsComponent } from './redeem-records.component';

@NgModule({
    imports: [CommonModule, FormsModule, SharedModule, RedeemRecordsRoutingModule, RedeemRecordsComponent],
})
export class RedeemRecordsModule {}
