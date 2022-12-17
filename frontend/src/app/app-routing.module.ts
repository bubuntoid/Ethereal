import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateComponent } from './components/create/create.component';
import { JobComponent } from './components/job/job.component';

const routes: Routes = [ 
  { path: 'job/:id', component: JobComponent },
  { path: '', component: CreateComponent },
  { path: '**', redirectTo: ''},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
