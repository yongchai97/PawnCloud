import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './service-proxies';

export class CustomerLookupDto {
    id: number = 0;
    displayName: string = '';
}

export class PawnTicketLookupDto {
    id: number = 0;
    displayName: string = '';
}

export class CustomerDocumentSummaryDto {
    id: number = 0;
    customer: number = 0;
    documentType?: string;
    fileName?: string;
    creationTime?: string;
}

export class LookupListResultDto<T> {
    items: T[] = [];
}

export interface CreatePawnTicketWithItemsDto {
    ticket: any;
    items: any[];
}

@Injectable()
export class LookupServiceProxy {
    constructor(
        private http: HttpClient,
        @Optional() @Inject(API_BASE_URL) private baseUrl?: string
    ) {}

    private url(path: string): string {
        return (this.baseUrl ?? '') + '/api/services/app' + path;
    }

    getCustomersForLookup(filter?: string): Observable<LookupListResultDto<CustomerLookupDto>> {
        let params = new HttpParams();
        if (filter) {
            params = params.set('Filter', filter);
        }
        return this.http.get<LookupListResultDto<CustomerLookupDto>>(
            this.url('/Customer/GetCustomersForLookup'),
            { params }
        );
    }

    getPawnTicketsForLookup(): Observable<LookupListResultDto<PawnTicketLookupDto>> {
        return this.http.get<LookupListResultDto<PawnTicketLookupDto>>(
            this.url('/PawnTicket/GetPawnTicketsForLookup')
        );
    }

    getCustomerDocumentsByCustomerId(customerId: number): Observable<LookupListResultDto<CustomerDocumentSummaryDto>> {
        const params = new HttpParams().set('Id', customerId.toString());
        return this.http.get<LookupListResultDto<CustomerDocumentSummaryDto>>(
            this.url('/CustomerDocument/GetByCustomerId'),
            { params }
        );
    }

    // Backend CreateOrEdit now returns the entity id (int).
    // The NSwag-generated proxies still declare createOrEdit as Observable<void>, so these
    // wrappers re-issue the same POST and parse the int response.
    createCustomer(input: any): Observable<number> {
        return this.http.post<number>(this.url('/Customer/CreateOrEdit'), input);
    }

    createCustomerDocument(input: any): Observable<number> {
        return this.http.post<number>(this.url('/CustomerDocument/CreateOrEdit'), input);
    }

    createPawnTicket(input: any): Observable<number> {
        return this.http.post<number>(this.url('/PawnTicket/CreateOrEdit'), input);
    }

    createPawnItem(input: any): Observable<number> {
        return this.http.post<number>(this.url('/PawnItem/CreateOrEdit'), input);
    }

    createLoan(input: any): Observable<number> {
        return this.http.post<number>(this.url('/Loan/CreateOrEdit'), input);
    }

    createPawnTicketWithItems(input: CreatePawnTicketWithItemsDto): Observable<number> {
        return this.http.post<number>(
            this.url('/PawnTicket/CreateWithItems'),
            input
        );
    }

    getPawnItemsByPawnTicketId(pawnTicketId: number): Observable<LookupListResultDto<any>> {
        const params = new HttpParams().set('Id', pawnTicketId.toString());
        return this.http.get<LookupListResultDto<any>>(
            this.url('/PawnItem/GetByPawnTicketId'),
            { params }
        );
    }

    deletePawnItem(id: number): Observable<void> {
        const params = new HttpParams().set('Id', id.toString());
        return this.http.delete<void>(
            this.url('/PawnItem/Delete'),
            { params }
        );
    }
}