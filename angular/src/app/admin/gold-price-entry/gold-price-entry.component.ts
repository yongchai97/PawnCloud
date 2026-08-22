import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import moment from 'moment';
import { finalize } from 'rxjs/operators';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    DailyGoldPriceDetailDto,
    DailyGoldPriceInputDto,
    DailyGoldPriceServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './gold-price-entry.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, ButtonModule, TableModule],
    animations: [appModuleAnimation()],
})
export class GoldPriceEntryComponent extends AppComponentBase implements OnInit {
    selectedDate = moment().format('YYYY-MM-DD');
    basePrice: number | null = null;
    records: DailyGoldPriceDetailDto[] = [];
    editingRecord: DailyGoldPriceDetailDto | null = null;
    editingRecordId: number | undefined;
    loading = false;
    saving = false;

    constructor(
        injector: Injector,
        private dailyGoldPriceService: DailyGoldPriceServiceProxy,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadRecords();
    }

    loadRecords(): void {
        if (!this.selectedDate) {
            return;
        }

        this.loading = true;
        this.dailyGoldPriceService
            .getAll(moment(this.selectedDate, 'YYYY-MM-DD'))
            .pipe(
                finalize(() => {
                    this.loading = false;
                    this.cd.detectChanges();
                })
            )
            .subscribe((records) => {
                this.records = records || [];
                this.editingRecord = null;
                this.editingRecordId = undefined;
            });
    }

    keyDailyGoldPrice(): void {
        if (this.basePrice === null || this.basePrice < 0 || !this.selectedDate) {
            this.notify.warn('Enter a valid daily gold price.');
            return;
        }

        const input = new DailyGoldPriceInputDto({
            todayDate: moment(this.selectedDate, 'YYYY-MM-DD'),
            price: this.basePrice,
        });

        this.saving = true;
        this.dailyGoldPriceService
            .keyDailyGoldPrice(input)
            .pipe(finalize(() => (this.saving = false)))
            .subscribe((message) => {
                this.notify.success(message || 'Daily gold price updated.');
                this.loadRecords();
            });
    }

    startEditing(record: DailyGoldPriceDetailDto): void {
        this.editingRecord = record.clone();
        this.editingRecordId = record.id;
    }

    cancelEditing(): void {
        this.editingRecord = null;
        this.editingRecordId = undefined;
    }

    saveRecord(): void {
        if (!this.editingRecord) {
            return;
        }

        this.saving = true;
        this.dailyGoldPriceService
            .createOrEdit(this.editingRecord)
            .pipe(finalize(() => (this.saving = false)))
            .subscribe(() => {
                this.notify.success('Daily gold price record saved.');
                this.loadRecords();
            });
    }

    isEditing(record: DailyGoldPriceDetailDto): boolean {
        return this.editingRecordId !== undefined && this.editingRecordId === record.id;
    }

    formatDate(date: moment.Moment): string {
        return date ? date.format('YYYY-MM-DD') : '';
    }
}