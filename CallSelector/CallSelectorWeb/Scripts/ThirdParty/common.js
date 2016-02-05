 function setClick(selector, func) {
     $(selector).unbind('click');
     $(selector).click(func);
 }

 function setHtml(obj, selector, val) {
     var o = obj.find(selector);
     if (null != o) o.html(val);
     return o;
 }


 