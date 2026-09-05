import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CreateOrEditItemListingDto, ItemListing, ItemListingServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
    templateUrl: './item-listings.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, ButtonModule, TableModule],
    animations: [appModuleAnimation()],
})
export class ItemListingsComponent extends AppComponentBase implements OnInit {
    records: ItemListing[] = [];
    form = new CreateOrEditItemListingDto();
    editing = false;
    saving = false;
    loading = false;

    constructor(
        injector: Injector,
        private itemListingService: ItemListingServiceProxy,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadRecords();
    }

    loadRecords(): void {
        this.loading = true;
        this.itemListingService
            .getAll()
            .pipe(
                finalize(() => {
                    this.loading = false;
                    this.cd.detectChanges();
                })
            )
            .subscribe(
                (result) => {
                    this.records = result || [];
                    this.cd.detectChanges();
                },
                () => {
                    this.notify.error('Unable to load item listings.');
                    this.records = [];
                }
            );
    }

    newListing(): void {
        this.form = new CreateOrEditItemListingDto();
        this.editing = true;
    }

    editListing(record: ItemListing): void {
        this.form = new CreateOrEditItemListingDto({
            id: record.id,
            code: record.code,
            description: record.description,
            category: record.category,
        });
        this.editing = true;
    }

    cancel(): void {
        this.editing = false;
    }

    save(): void {
        if (!this.form.code?.trim() || !this.form.description?.trim() || !this.form.category?.trim()) {
            this.notify.warn('Enter valid item listing values.');
            return;
        }

        this.saving = true;
        this.itemListingService.createOrEdit(this.form).pipe(finalize(() => (this.saving = false))).subscribe(() => {
            this.notify.success('Item listing saved.');
            this.editing = false;
            this.loadRecords();
        });
    }

    deactivate(record: ItemListing): void {
        if (!record.id || !confirm(`Deactivate ${record.code}?`)) return;
        this.itemListingService.deactiveItemListing(record.id).subscribe(() => {
            this.notify.success('Item listing deactivated.');
            this.loadRecords();
        });
    }
}
