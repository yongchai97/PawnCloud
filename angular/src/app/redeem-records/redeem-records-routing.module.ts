import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { RedeemRecordsComponent } from './redeem-records.component';

@NgModule({
    imports: [RouterModule.forChild([{ path: '', component: RedeemRecordsComponent }])],
    exports: [RouterModule],
})
export class RedeemRecordsRoutingModule {}
