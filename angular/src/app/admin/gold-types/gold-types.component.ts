import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { GoldTypeServiceProxy, GoldType } from '@shared/service-proxies/service-proxies';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateGoldTypeDialogComponent } from './create-gold-type/create-gold-type-dialog.component';
import { EditGoldTypeDialogComponent } from './edit-gold-type/edit-gold-type-dialog.component';

@Component({
    templateUrl: './gold-types.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [CommonModule, FormsModule, TableModule, ButtonModule, LocalizePipe],
})
export class GoldTypesComponent extends AppComponentBase implements OnInit {
    keyword = '';
    records: GoldType[] = [];
    loading = false;

    constructor(
        injector: Injector,
        private _goldTypeService: GoldTypeServiceProxy,
        private _modalService: BsModalService,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadRecords();
    }

    loadRecords(): void {
        this.loading = true;
        this._goldTypeService
            .getAll()
            .pipe(
                finalize(() => {
                    this.loading = false;
                    this.cd.detectChanges();
                })
            )
            .subscribe((result: GoldType[]) => {
                this.records = result || [];
                this.cd.detectChanges();
            }, () => {
                this.records = [];
                this.notify.error('Unable to load gold types.');
            });
    }

    toggleStatus(entity: GoldType): void {
        if (!entity.id) return;
        const action = entity.active ? 'Deactivate' : 'Activate';
        abp.message.confirm(`${action} ${entity.purity}?`, undefined, (result: boolean) => {
            if (result) {
                this._goldTypeService.deactiveGoldType(entity.id).subscribe(() => {
                    this.notify.success(`Gold type ${entity.active ? 'deactivated' : 'activated'}.`);
                    this.loadRecords();
                });
            }
        });
    }

    newGoldType(): void {
        const dialog: BsModalRef = this._modalService.show(CreateGoldTypeDialogComponent, { class: 'modal-lg' });
        dialog.content.onSave.subscribe(() => this.loadRecords());
    }

    editGoldType(entity: GoldType): void {
        const dialog: BsModalRef = this._modalService.show(EditGoldTypeDialogComponent, {
            class: 'modal-lg',
            initialState: { id: entity.id },
        });
        dialog.content.onSave.subscribe(() => this.loadRecords());
    }
}
