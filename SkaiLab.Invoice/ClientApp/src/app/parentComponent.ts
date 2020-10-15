import $ from 'jquery';
import { Component, Inject, Injectable, NgModule } from "@angular/core";

@NgModule({})
export class ParentComponent {
    constructor(){
        this.hideProgressBar();
    }
    hideProgressBar(){
        $(".loading").hide();
    }
    showProgressBar(){
        $(".loading").show();
    }
  }
  