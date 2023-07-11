/*

@license
dhtmlxScheduler.Net v.4.0.3 Professional Evaluation

This software is covered by DHTMLX Evaluation License. Contact sales@dhtmlx.com to get Commercial or Enterprise license. Usage without proper license is prohibited.

(c) XB Software Ltd.

*/
Scheduler.plugin(function(e){!function(){function t(e,t,a){var i=e+"="+a+(t?"; "+t:"");document.cookie=i}function a(e){var t=e+"=";if(document.cookie.length>0){var a=document.cookie.indexOf(t);if(-1!=a){a+=t.length;var i=document.cookie.indexOf(";",a);return-1==i&&(i=document.cookie.length),document.cookie.substring(a,i)}}return""}function i(e){return(e._obj.id||"scheduler")+"_settings"}var n=!0;e.attachEvent("onBeforeViewChange",function(t,r,o,s){if(n&&e._get_url_nav){var d=e._get_url_nav()
;(d.date||d.mode||d.event)&&(n=!1)}var l=i(e);if(n){n=!1;var _=a(l);if(_){e._min_date||(e._min_date=s),_=unescape(_).split("@"),_[0]=this._helpers.parseDate(_[0]);var c=this.isViewExists(_[1])?_[1]:o,s=isNaN(+_[0])?s:_[0];return window.setTimeout(function(){e.setCurrentView(s,c)},1),!1}}return!0}),e.attachEvent("onViewChange",function(a,n){t(i(e),"expires=Sun, 31 Jan 9999 22:00:00 GMT",escape(this._helpers.formatDate(n)+"@"+a))});var r=e._load;e._load=function(){var t=arguments
;if(e._date)r.apply(this,t);else{var a=this;window.setTimeout(function(){r.apply(a,t)},1)}}}()});