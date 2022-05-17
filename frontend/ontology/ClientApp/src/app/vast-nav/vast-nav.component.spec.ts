import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VastNavComponent } from './vast-nav.component';

describe('VastNavComponent', () => {
  let component: VastNavComponent;
  let fixture: ComponentFixture<VastNavComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VastNavComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(VastNavComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
