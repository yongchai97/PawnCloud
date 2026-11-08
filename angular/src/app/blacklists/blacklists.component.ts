import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { AppComponentBase } from '@shared/app-component-base';
import { CustomerLookupDto, LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import {
    CreateOrEditGlobalBlackListDto,
    CreateOrEditGlobalBlackListRequestDto,
    CreateOrEditPrivateBlackListDto,
    GlobalBlackList,
    GlobalBlackListRequest,
    GlobalBlackListRequestServiceProxy,
    GlobalBlackListServiceProxy,
    PrivateBlackList,
    PrivateBlackListServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './blacklists.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizePipe, ButtonModule, DropdownModule, InputTextModule],
})
export class BlacklistsComponent extends AppComponentBase implements OnInit {
    customers: CustomerLookupDto[] = [];
    privateBlackLists: PrivateBlackList[] = [];
    globalBlackLists: GlobalBlackList[] = [];
    globalRequests: GlobalBlackListRequest[] = [];

    privateForm = new CreateOrEditPrivateBlackListDto({ id: undefined, fullName: '', banReason: '' });
    requestForm = new CreateOrEditGlobalBlackListRequestDto({ id: undefined, fullName: '', banReason: '', status: 'pending' });
    globalForm = new CreateOrEditGlobalBlackListDto({ id: undefined, fullName: '', banReason: '' });
    pageType: 'private' | 'requests' | 'global' = 'private';
    privateEditing = false;
    globalEditing = false;
    savingPrivate = false;
    savingRequest = false;
    savingGlobal = false;
    deletingPrivateId?: number;
    deletingRequestId?: number;
    deletingGlobalId?: number;
    editingRequestId?: number;
    savingRequestId?: number;
    isHost = false;

    constructor(
        injector: Injector,
        private lookupService: LookupServiceProxy,
        private privateBlackListService: PrivateBlackListServiceProxy,
        private globalBlackListService: GlobalBlackListServiceProxy,
        private globalRequestService: GlobalBlackListRequestServiceProxy,
        private cd: ChangeDetectorRef,
        private route: ActivatedRoute
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.pageType = this.route.snapshot.data['blacklistType'] || 'private';
        this.isHost = this.permission.isGranted('Pages.GlobalBlackLists');
        if (this.pageType !== 'global') {
            this.loadCustomers();
        }
        if (this.pageType === 'private') {
            this.loadPrivateBlackLists();
        } else if (this.pageType === 'requests') {
            this.loadGlobalRequests();
        } else if (this.pageType === 'global') {
            this.loadGlobalBlackLists();
        }
    }

    refresh(): void {
        if (this.pageType === 'private') {
            this.loadPrivateBlackLists();
        } else if (this.pageType === 'requests') {
            this.loadGlobalRequests();
        } else if (this.pageType === 'global') {
            this.loadGlobalBlackLists();
        }
    }

    loadCustomers(): void {
        this.lookupService.getCustomersForLookup('').subscribe((result) => {
            this.customers = result.items || [];
            this.cd.detectChanges();
        });
    }

    loadPrivateBlackLists(): void {
        this.privateBlackListService.getAll().subscribe({
            next: (result) => {
                this.privateBlackLists = result || [];
                this.cd.detectChanges();
            },
            error: (error) => this.showError(error, 'Unable to load private blacklist records.'),
        });
    }

    loadGlobalBlackLists(): void {
        this.globalBlackListService.getAll().subscribe({
            next: (result) => {
                this.globalBlackLists = result || [];
                this.cd.detectChanges();
            },
            error: (error) => this.showError(error, 'Unable to load global blacklist records.'),
        });
    }

    loadGlobalRequests(): void {
        if (this.isHost) {
            this.globalBlackListService.getAllRequestFromAllTenant().subscribe({
                next: (result) => {
                    this.globalRequests = result || [];
                    this.cd.detectChanges();
                },
                error: (error) => this.showError(error, 'Unable to load global blacklist requests.'),
            });
            return;
        }

        this.globalRequestService.getAll().subscribe({
            next: (result) => {
                this.globalRequests = result || [];
                this.cd.detectChanges();
            },
            error: (error) => this.showError(error, 'Unable to load global blacklist requests.'),
        });
    }

    savePrivate(): void {
        if (!this.privateForm.fullName?.trim() || !this.privateForm.banReason?.trim()) {
            this.notify.warn('Full name and ban reason are required.');
            return;
        }

        this.savingPrivate = true;
        this.privateForm.fullName = this.privateForm.fullName.trim();
        this.privateForm.banReason = this.privateForm.banReason.trim();
        this.privateBlackListService.createOrEdit(this.privateForm).subscribe({
            next: () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.savingPrivate = false;
                this.resetPrivateForm();
                this.loadPrivateBlackLists();
            },
            error: (error) => {
                this.savingPrivate = false;
                this.showError(error, 'Unable to save private blacklist record.');
            },
        });
    }

    editPrivate(record: PrivateBlackList): void {
        this.privateEditing = true;
        this.privateForm = new CreateOrEditPrivateBlackListDto({
            id: record.id,
            fullName: record.fullName,
            banReason: record.banReason,
        });
    }

    resetPrivateForm(): void {
        this.privateEditing = false;
        this.privateForm = new CreateOrEditPrivateBlackListDto({ id: undefined, fullName: '', banReason: '' });
    }

    saveGlobal(): void {
        if (!this.globalForm.fullName?.trim() || !this.globalForm.banReason?.trim()) {
            this.notify.warn('Full name and ban reason are required.');
            return;
        }

        this.savingGlobal = true;
        this.globalForm.fullName = this.globalForm.fullName.trim();
        this.globalForm.banReason = this.globalForm.banReason.trim();
        this.globalBlackListService.createOrEdit(this.globalForm).subscribe({
            next: () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.savingGlobal = false;
                this.resetGlobalForm();
                this.loadGlobalBlackLists();
            },
            error: (error) => {
                this.savingGlobal = false;
                this.showError(error, 'Unable to save global blacklist record.');
            },
        });
    }

    editGlobal(record: GlobalBlackList): void {
        this.globalEditing = true;
        this.globalForm = new CreateOrEditGlobalBlackListDto({
            id: record.id,
            fullName: record.fullName,
            banReason: record.banReason,
        });
    }

    resetGlobalForm(): void {
        this.globalEditing = false;
        this.globalForm = new CreateOrEditGlobalBlackListDto({ id: undefined, fullName: '', banReason: '' });
    }

    deletePrivate(record: PrivateBlackList): void {
        if (!record.id || !confirm(`Delete ${record.fullName || 'this private blacklist record'}?`)) {
            return;
        }

        this.deletingPrivateId = record.id;
        this.privateBlackListService.delete(record.id).subscribe({
            next: () => {
                this.deletingPrivateId = undefined;
                this.resetPrivateForm();
                this.loadPrivateBlackLists();
            },
            error: (error) => {
                this.deletingPrivateId = undefined;
                this.showError(error, 'Unable to delete private blacklist record.');
            },
        });
    }

    saveRequest(): void {
        if (!this.requestForm.fullName?.trim() || !this.requestForm.banReason?.trim()) {
            this.notify.warn('Full name and ban reason are required.');
            return;
        }

        this.savingRequest = true;
        this.requestForm.fullName = this.requestForm.fullName.trim();
        this.requestForm.banReason = this.requestForm.banReason.trim();
        this.requestForm.status = 'pending';
        this.globalRequestService.createOrEdit(this.requestForm).subscribe({
            next: () => {
                this.notify.info('Global blacklist request submitted.');
                this.savingRequest = false;
                this.requestForm = new CreateOrEditGlobalBlackListRequestDto({ id: undefined, fullName: '', banReason: '', status: 'pending' });
                this.loadGlobalRequests();
            },
            error: (error) => {
                this.savingRequest = false;
                this.showError(error, 'Unable to submit global blacklist request.');
            },
        });
    }

    startRequestEdit(request: GlobalBlackListRequest): void {
        this.editingRequestId = request.id;
    }

    cancelRequestEdit(): void {
        this.editingRequestId = undefined;
        this.loadGlobalRequests();
    }

    deleteRequest(request: GlobalBlackListRequest): void {
        if (!request.id || !confirm(`Delete ${request.fullName || 'this global blacklist request'}?`)) {
            return;
        }

        this.deletingRequestId = request.id;
        this.globalRequestService.delete(request.id).subscribe({
            next: () => {
                this.deletingRequestId = undefined;
                this.editingRequestId = undefined;
                this.loadGlobalRequests();
            },
            error: (error) => {
                this.deletingRequestId = undefined;
                this.showError(error, 'Unable to delete global blacklist request.');
            },
        });
    }

    saveRequestDecision(request: GlobalBlackListRequest): void {
        if (!request.id || !request.fullName?.trim() || !request.banReason?.trim()) {
            this.notify.warn('Full name and ban reason are required.');
            return;
        }

        this.savingRequestId = request.id;
        const input = new CreateOrEditGlobalBlackListRequestDto({
            id: request.id,
            fullName: request.fullName.trim(),
            banReason: request.banReason.trim(),
            status: request.status || 'pending',
        });
        this.globalBlackListService.convertTenantRequestToGlobalBlackList(input).subscribe({
            next: () => {
                this.notify.info('Global blacklist request updated.');
                this.savingRequestId = undefined;
                this.editingRequestId = undefined;
                this.loadGlobalRequests();
                if (input.status?.toLowerCase() === 'approved') {
                    this.loadGlobalBlackLists();
                }
            },
            error: (error) => {
                this.savingRequestId = undefined;
                this.showError(error, 'Unable to update global blacklist request.');
            },
        });
    }

    deleteGlobal(record: GlobalBlackList): void {
        if (!record.id || !confirm(`Delete ${record.fullName || 'this global blacklist record'}?`)) {
            return;
        }

        this.deletingGlobalId = record.id;
        this.globalBlackListService.delete(record.id).subscribe({
            next: () => {
                this.deletingGlobalId = undefined;
                this.loadGlobalBlackLists();
            },
            error: (error) => {
                this.deletingGlobalId = undefined;
                this.showError(error, 'Unable to delete global blacklist record.');
            },
        });
    }

    private showError(error: any, fallback: string): void {
        const message = error?.error?.error?.message || error?.error?.message || error?.message || fallback;
        this.notify.error(message);
        this.cd.detectChanges();
    }
}
