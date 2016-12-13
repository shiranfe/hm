!function($) {

    "use strict";

    var Selectpicker = function(element, options, e) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        this.$element = $(element);
        this.$newElement = null;
        this.button = null;

        //Merge defaults, options and data-attributes to make our options
        this.options = $.extend({}, $.fn.selectpicker.defaults, this.$element.data(), typeof options == "object" && options);

        //If we have no title yet, check the attribute 'title' (this is missed by jq as its not a data-attribute
        if(this.options.title==null)
            this.options.title = this.$element.attr("title");

        //Expose public methods
        this.val = Selectpicker.prototype.val;
        this.render = Selectpicker.prototype.render;
        this.refresh = Selectpicker.prototype.refresh;
        this.selectAll = Selectpicker.prototype.selectAll;
        this.deselectAll = Selectpicker.prototype.deselectAll;
        this.init();
    };

    Selectpicker.prototype = {

        constructor: Selectpicker,

        init: function (e) {
            if (!this.options.container) {
                this.$element.hide();
            } else {
                this.$element.css("visibility","hidden");
            };
            this.multiple = this.$element.prop("multiple");
            var id = this.$element.attr("id");
            this.$newElement = this.createView()
            this.$element.after(this.$newElement);

            if (this.options.container) {
                this.selectPosition();
            }
            this.button = this.$newElement.find("> button");
            if (id !== undefined) {
                var _this = this;
                this.button.attr("data-id", id);
                $("label[for=\"" + id + "\"]").click(function(){
                    _this.button.focus();
                })
            }
            if (this.$element.attr("class")) {
                this.$newElement.addClass(this.$element.attr("class").replace(/selectpicker/gi, ""));
            }

            //If we are multiple, then add the show-tick class by default
            if(this.multiple) {
                 this.$newElement.addClass("show-tick");
            }
            this.button.addClass(this.options.style);
            this.checkDisabled();
            this.checkTabIndex();
            this.clickListener();

            this.render();
            this.setSize();
        },

        createDropdown: function() {
            var drop =
                "<div class='btn-group bootstrap-select'>" +
                    "<button type='button' class='btn dropdown-toggle' data-toggle='dropdown'>" +
                        "<div class='filter-option pull-left'></div>&nbsp;" +
                        "<div class='caret'></div>" +
                    "</button>" +
                    "<ul class='dropdown-menu' role='menu'>" +
                    "</ul>" +
                "</div>";

            return $(drop);
        },


        createView: function() {
            var $drop = this.createDropdown();
            var $li = this.createLi();
            $drop.find("ul").append($li);
            return $drop;
        },

        reloadLi: function() {
            //Remove all children.
            this.destroyLi();
            //Re build
            var $li = this.createLi();
            this.$newElement.find("ul").append( $li );
        },

        destroyLi:function() {
            this.$newElement.find("li").remove();
        },

        createLi: function() {

            var _this = this;
            var _liA = [];
            var _liHtml = "";


            this.$element.find("option").each(function(index) {
                var $this = $(this);

                //Get the class and text for the option
                var optionClass = $this.attr("class") || "";
                var text =  $this.html();
                var subtext = $this.data("subtext") !== undefined ? "<small class=\"muted\">" + $this.data("subtext") + "</small>" : "";
                var icon = $this.data("icon") !== undefined ? "<i class=\""+$this.data("icon")+"\"></i> " : "";
                if (icon !== "" && ($this.is(":disabled") || $this.parent().is(":disabled"))) {
                    icon = "<span>"+icon+"</span>";
                }

                //Prepend any icon and append any subtext to the main text.
                text = icon + "<span class=\"text\">" + text + subtext + "</span>";

                if (_this.options.hideDisabled && ($this.is(":disabled") || $this.parent().is(":disabled"))) {
                    _liA.push("<a style=\"min-height: 0; padding: 0\"></a>");
                } else if ($this.parent().is("optgroup") && $this.data("divider") != true) {
                    if ($this.index() == 0) {
                        //Get the opt group label
                        var label = $this.parent().attr("label");
                        var labelSubtext = $this.parent().data("subtext") !== undefined ? "<small class=\"muted\">"+$this.parent().data("subtext")+"</small>" : "";
                        var labelIcon = $this.parent().data("icon") ? "<i class=\""+$this.parent().data("icon")+"\"></i> " : "";
                        label = labelIcon + "<span class=\"text\">" + label + labelSubtext + "</span>";

                        if ($this[0].index != 0) {
                            _liA.push(
                                "<div class=\"div-contain\"><div class=\"divider\"></div></div>"+
                                "<dt>"+label+"</dt>"+
                                _this.createA(text, "opt " + optionClass )
                                );
                        } else {
                            _liA.push(
                                "<dt>"+label+"</dt>"+
                                _this.createA(text, "opt " + optionClass ));
                        }
                    } else {
                         _liA.push( _this.createA(text, "opt " + optionClass )  );
                    }
                } else if ($this.data("divider") == true) {
                    _liA.push("<div class=\"div-contain\"><div class=\"divider\"></div></div>");
                } else if ($(this).data("hidden") == true) {
                    _liA.push("");
                } else {
                    _liA.push( _this.createA(text, optionClass ) );
                }
            });

            $.each(_liA, function(i, item){
                _liHtml += "<li rel=" + i + ">" + item + "</li>";
            });

            //If we are not multiple, and we dont have a selected item, and we dont have a title, select the first element so something is set in the button
            if(!this.multiple && this.$element.find("option:selected").length==0 && !_this.options.title) {
                this.$element.find("option").eq(0).prop("selected", true).attr("selected", "selected");
            }

            return $(_liHtml);
        },

        createA:function(text, classes) {
         return "<a tabindex=\"0\" class=\""+classes+"\">" +
                 text +
                 "<i class=\"icon-ok check-mark\"></i>" +
                 "</a>";
        },

        render:function() {
            var _this = this;

            //Update the LI to match the SELECT
            this.$element.find("option").each(function(index) {
               _this.setDisabled(index, $(this).is(":disabled") || $(this).parent().is(":disabled") );
               _this.setSelected(index, $(this).is(":selected") );
            });

            var selectedItems = this.$element.find("option:selected").map(function(index,value) {
                var subtext;
                if (_this.options.showSubtext && $(this).attr("data-subtext") && !_this.multiple) {
                    subtext = " <small class=\"muted\">"+$(this).data("subtext") +"</small>";
                } else {
                    subtext = "";
                }
                if($(this).attr("title")!=undefined) {
                    return $(this).attr("title");
                } else {
                    return $(this).text() + subtext;
                }
            }).toArray();

            //Fixes issue in IE10 occurring when no default option is selected and at least one option is disabled
            //Convert all the values into a comma delimited string
            var title = !this.multiple ? selectedItems[0] : selectedItems.join(", ");

            //If this is multi select, and the selectText type is count, the show 1 of 2 selected etc..
            if(_this.multiple && _this.options.selectedTextFormat.indexOf("count") > -1) {
                var max = _this.options.selectedTextFormat.split(">");
                var notDisabled = this.options.hideDisabled ? ":not([disabled])" : "";
                if( (max.length>1 && selectedItems.length > max[1]) || (max.length==1 && selectedItems.length>=2)) {
                    title = _this.options.countSelectedText.replace("{0}", selectedItems.length).replace("{1}", this.$element.find("option:not([data-divider=\"true\"]):not([data-hidden=\"true\"])"+notDisabled).length);
                }
             }

            //If we dont have a title, then use the default, or if nothing is set at all, use the not selected text
            if(!title) {
                title = _this.options.title != undefined ? _this.options.title : _this.options.noneSelectedText;
            }

            var subtext;
            if (this.options.showSubtext && this.$element.find("option:selected").attr("data-subtext")) {
                subtext = " <small class=\"muted\">"+this.$element.find("option:selected").data("subtext") +"</small>";
            } else {
                subtext = "";
            }

            var icon = this.$element.find("option:selected").data("icon") || "";
            if(icon.length) {
                icon = "<i class=\"" + icon + "\"></i> ";
            }

            _this.$newElement.find(".filter-option").html(icon + title + subtext);
        },

        setSize:function() {
            if(this.options.container) {
                // Show $newElement before perfoming size calculations
                this.$newElement.toggle(this.$element.parent().is(":visible"));
            }
            var _this = this;
            var menu = this.$newElement.find(".dropdown-menu");
            var menuA = menu.find("li > a");
            var liHeight = this.$newElement.addClass("open").find(".dropdown-menu li > a").outerHeight();
            this.$newElement.removeClass("open");
            var divHeight = menu.find("li .divider").outerHeight(true);
            var selectOffset_top = this.$newElement.offset().top;
            var selectHeight = this.$newElement.outerHeight();
            var menuPadding = parseInt(menu.css("padding-top")) + parseInt(menu.css("padding-bottom")) + parseInt(menu.css("border-top-width")) + parseInt(menu.css("border-bottom-width"));
            var notDisabled = this.options.hideDisabled ? ":not(.disabled)" : "";
            var menuHeight;
            if (this.options.size == "auto") {
                var getSize = function() {
                    var selectOffset_top_scroll = selectOffset_top - $(window).scrollTop();
                    var windowHeight = window.innerHeight;
                    var menuExtras = menuPadding + parseInt(menu.css("margin-top")) + parseInt(menu.css("margin-bottom")) + 2;
                    var selectOffset_bot = windowHeight - selectOffset_top_scroll - selectHeight - menuExtras;
                    var minHeight;
                    menuHeight = selectOffset_bot;
                    if (_this.$newElement.hasClass("dropup")) {
                        menuHeight = selectOffset_top_scroll - menuExtras;
                    }
                    if ((menu.find("li").length + menu.find("dt").length) > 3) {
                        minHeight = liHeight*3 + menuExtras - 2;
                    } else {
                        minHeight = 0;
                    }
                    menu.css({'max-height' : menuHeight + "px", 'overflow-y' : "auto", 'min-height' : minHeight + "px"});
            }
                getSize();
                $(window).resize(getSize);
                $(window).scroll(getSize);
            } else if (this.options.size && this.options.size != "auto" && menu.find("li"+notDisabled).length > this.options.size) {
                var optIndex = menu.find("li"+notDisabled+" > *").filter(":not(.div-contain)").slice(0,this.options.size).last().parent().index();
                var divLength = menu.find("li").slice(0,optIndex + 1).find(".div-contain").length;
                menuHeight = liHeight*this.options.size + divLength*divHeight + menuPadding;
                menu.css({'max-height' : menuHeight + "px", 'overflow-y' : "auto"});
            }

            //Set width of select
            if (this.options.width == "auto") {
                this.$newElement.find(".dropdown-menu").css("min-width","0");
                var ulWidth = this.$newElement.find(".dropdown-menu").css("width");
                this.$newElement.css("width",ulWidth);
                if (this.options.container) {
                    this.$element.css("width",ulWidth);
                }
            } else if (this.options.width) {
                if (this.options.container) {
                    // Note: options.width can be %
                    this.$element.css("width", this.options.width);
                    // Set pixel width of $newElement based on $element's pixel width
                    this.$newElement.width(this.$element.outerWidth());
                } else {
                    this.$newElement.css("width",this.options.width);
                }
            } else if(this.options.container) {
                // Set width of $newElement based on $element
                this.$newElement.width(this.$element.outerWidth());
            }
        },

        selectPosition:function() {
            var containerOffset = $(this.options.container).offset();
            var eltOffset = this.$element.offset();
            if(containerOffset && eltOffset) {
                var selectElementTop = eltOffset.top - containerOffset.top;
                var selectElementLeft = eltOffset.left - containerOffset.left;
                this.$newElement.appendTo(this.options.container);
                this.$newElement.css({'position':"absolute", 'top':selectElementTop+"px", 'left':selectElementLeft+"px"});
            }
        },

        refresh:function() {
            this.reloadLi();
            this.render();
            this.setSize();
            this.checkDisabled();
            if (this.options.container) {
                this.selectPosition();
            }
        },

        setSelected:function(index, selected) {
            if(selected) {
                this.$newElement.find("li").eq(index).addClass("selected");
            } else {
                this.$newElement.find("li").eq(index).removeClass("selected");
            }
        },

        setDisabled:function(index, disabled) {
            if(disabled) {
                this.$newElement.find("li").eq(index).addClass("disabled").find("a").attr("href","#").attr("tabindex",-1);
            } else {
                this.$newElement.find("li").eq(index).removeClass("disabled").find("a").removeAttr("href").attr("tabindex",0);
            }
        },

        isDisabled: function() {
            return this.$element.is(":disabled") || this.$element.attr("readonly");
        },

        checkDisabled: function() {
            if (this.isDisabled()) {
                this.button.addClass("disabled");
                this.button.click(function(e) {
                    e.preventDefault();
                });
                this.button.attr("tabindex","-1");
            } else if (!this.isDisabled() && this.button.hasClass("disabled")) {
                this.button.removeClass("disabled");
                this.button.click(function() {
                    return true;
                });
                this.button.removeAttr("tabindex");
            }
        },

        checkTabIndex: function() {
            if (this.$element.is("[tabindex]")) {
                var tabindex = this.$element.attr("tabindex");
                this.button.attr("tabindex", tabindex);
            }
        },

        clickListener: function() {
            var _this = this;

            $("body").on("touchstart.dropdown", ".dropdown-menu", function (e) { e.stopPropagation(); });

            this.$newElement.on("click", "li a", function(e){
                var clickedIndex = $(this).parent().index(),
                    $this = $(this).parent(),
                    $select = $this.parents(".bootstrap-select"),
                    prevValue = _this.$element.val();

                //Dont close on multi choice menu
                if(_this.multiple) {
                    e.stopPropagation();
                }

                e.preventDefault();

                //Dont run if we have been disabled
                if (_this.$element.not(":disabled") && !$(this).parent().hasClass("disabled")){
                    //Deselect all others if not multi select box
                    if (!_this.multiple) {
                        _this.$element.find("option").prop("selected", false);
                        _this.$element.find("option").eq(clickedIndex).prop("selected", true);
                    }
                    //Else toggle the one we have chosen if we are multi select.
                    else {
                        var selected = _this.$element.find("option").eq(clickedIndex).prop("selected");

                        if(selected) {
                            _this.$element.find("option").eq(clickedIndex).prop("selected", false);
                        } else {
                            _this.$element.find("option").eq(clickedIndex).prop("selected", true);
                        }
                    }

                    $select.find("button").focus();

                    // Trigger select 'change'
                    if (prevValue != _this.$element.val()) {
                        _this.$element.trigger("change");
                    }

                    _this.render();
                }

            });

           this.$newElement.on("click", "li.disabled a, li dt, li .div-contain", function(e) {
                e.preventDefault();
                e.stopPropagation();
                var $select = $(this).parent().parents(".bootstrap-select");
                $select.find("button").focus();
            });

            this.$element.on("change", function(e) {
                _this.render();
            });
        },

        val:function(value) {

            if(value!=undefined) {
                this.$element.val( value );

                this.$element.trigger("change");
                return this.$element;
            } else {
                return this.$element.val();
            }
        },

        selectAll:function() {
            this.$element.find("option").prop("selected", true).attr("selected", "selected");
            this.render();
        },

        deselectAll:function() {
            this.$element.find("option").prop("selected", false).removeAttr("selected");
            this.render();
        },

        keydown: function (e) {
            var $this,
                $items,
                $parent,
                index,
                next,
                first,
                last,
                prev,
                nextPrev

            $this = $(this);

            $parent = $this.parent();

            $items = $("[role=menu] li:not(.divider):visible a", $parent);

            if (!$items.length) return;

            if (/(38|40)/.test(e.keyCode)) {

                index = $items.index($items.filter(":focus"));

                first = $items.parent(":not(.disabled)").first().index();
                last = $items.parent(":not(.disabled)").last().index();
                next = $items.eq(index).parent().nextAll(":not(.disabled)").eq(0).index();
                prev = $items.eq(index).parent().prevAll(":not(.disabled)").eq(0).index();
                nextPrev = $items.eq(next).parent().prevAll(":not(.disabled)").eq(0).index();

                if (e.keyCode == 38) {
                    if (index != nextPrev && index > prev) index = prev;
                    if (index < first) index = first;
                }

                if (e.keyCode == 40) {
                    if (index != nextPrev && index < next) index = next;
                    if (index > last) index = last;
                }

                $items.eq(index).focus()
            } else {
                var keyCodeMap = {
                    48:"0", 49:"1", 50:"2", 51:"3", 52:"4", 53:"5", 54:"6", 55:"7", 56:"8", 57:"9", 59:";",
                    65:"a", 66:"b", 67:"c", 68:"d", 69:"e", 70:"f", 71:"g", 72:"h", 73:"i", 74:"j", 75:"k", 76:"l",
                    77:"m", 78:"n", 79:"o", 80:"p", 81:"q", 82:"r", 83:"s", 84:"t", 85:"u", 86:"v", 87:"w", 88:"x", 89:"y", 90:"z",
                    96:"0", 97:"1", 98:"2", 99:"3", 100:"4", 101:"5", 102:"6", 103:"7", 104:"8", 105:"9"
                }

                var keyIndex = [];

                $items.each(function() {
                    if ($(this).parent().is(":not(.disabled)")) {
                        if ($.trim($(this).text().toLowerCase()).substring(0,1) == keyCodeMap[e.keyCode]) {
                            keyIndex.push($(this).parent().index());
                        }
                    }
                });

                var count = $(document).data("keycount");
                count++;
                $(document).data("keycount",count);

                var prevKey = $.trim($(":focus").text().toLowerCase()).substring(0,1);

                if (prevKey != keyCodeMap[e.keyCode]) {
                    count = 1;
                    $(document).data("keycount",count);
                } else if (count >= keyIndex.length) {
                    $(document).data("keycount",0);
                }

                $items.eq(keyIndex[count - 1]).focus();
            }

            if (/(13)/.test(e.keyCode)) {
                $(":focus").click();
                $parent.addClass("open");
                $(document).data("keycount",0);
            }
        },

        hide: function() {
            this.$newElement.hide();
        },

        show: function() {
            this.$newElement.show();
        }
    };

    $.fn.selectpicker = function(option, event) {
       //get the args of the outer function..
       var args = arguments;
       var value;
       var chain = this.each(function () {
            if ($(this).is("select")) {
                var $this = $(this),
                    data = $this.data("selectpicker"),
                    options = typeof option == "object" && option;

                if (!data) {
                    $this.data("selectpicker", (data = new Selectpicker(this, options, event)));
                } else if(options){
                    for(var i in options) {
                       data.options[i]=options[i];
                    }
                }

                if (typeof option == "string") {
                    //Copy the value of option, as once we shift the arguments
                    //it also shifts the value of option.
                    var property = option;
                    if(data[property] instanceof Function) {
                        [].shift.apply(args);
                        value = data[property].apply(data, args);
                    } else {
                        value = data.options[property];
                    }
                }
            }
        });

        if(value!=undefined) {
            return value;
        } else {
            return chain;
        }
    };

    $.fn.selectpicker.defaults = {
        style: null,
        size: "auto",
        title: null,
        selectedTextFormat : "values",
        noneSelectedText : "Nothing selected",
        countSelectedText: "{0} of {1} selected",
        width: null,
        container: false,
        hideDisabled: false,
        showSubtext: false
    }

    $(document)
        .data("keycount", 0)
        .on("keydown", "[data-toggle=dropdown], [role=menu]" , Selectpicker.prototype.keydown)

}(window.jQuery);


/* ===================================================
 * bootstrap-transition.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#transitions
 * ===================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================== */


!function ($) {

    "use strict"; // jshint ;_;


    /* CSS TRANSITION SUPPORT (http://www.modernizr.com/)
     * ======================================================= */

    $(function () {

        $.support.transition = (function () {

            var transitionEnd = (function () {

                var el = document.createElement("bootstrap")
                  , transEndEventNames = {
                      'WebkitTransition': "webkitTransitionEnd"
                    , 'MozTransition': "transitionend"
                    , 'OTransition': "oTransitionEnd otransitionend"
                    , 'transition': "transitionend"
                  }
                  , name

                for (name in transEndEventNames) {
                    if (el.style[name] !== undefined) {
                        return transEndEventNames[name]
                    }
                }

            }())

            return transitionEnd && {
                end: transitionEnd
            }

        })()

    })

}(window.jQuery);
/* =========================================================
 * bootstrap-modal.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#modals
 * =========================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================= */


!function ($) {

    "use strict"; // jshint ;_;


    /* MODAL CLASS DEFINITION
     * ====================== */

    var Modal = function (element, options) {
        this.options = options
        this.$element = $(element)
          .delegate("[data-dismiss=\"modal\"]", "click.dismiss.modal", $.proxy(this.hide, this))
        this.options.remote && this.$element.find(".modal-body").load(this.options.remote)
    }

    Modal.prototype = {

        constructor: Modal

      , toggle: function () {
          return this[!this.isShown ? "show" : "hide"]()
      }

      , show: function () {
          var that = this
            , e = $.Event("show")

          this.$element.trigger(e)

          if (this.isShown || e.isDefaultPrevented()) return

          this.isShown = true

          this.escape()

          this.backdrop(function () {
              var transition = $.support.transition && that.$element.hasClass("fade")

              if (!that.$element.parent().length) {
                  that.$element.appendTo(document.body) //don't move modals dom position
              }

              that.$element.show()

              if (transition) {
                  that.$element[0].offsetWidth // force reflow
              }

              that.$element
                .addClass("in")
                .attr("aria-hidden", false)

              that.enforceFocus()

              transition ?
                that.$element.one($.support.transition.end, function () { that.$element.focus().trigger("shown") }) :
                that.$element.focus().trigger("shown")

          })
      }

      , hide: function (e) {
          e && e.preventDefault()

          var that = this

          e = $.Event("hide")

          this.$element.trigger(e)

          if (!this.isShown || e.isDefaultPrevented()) return

          this.isShown = false

          this.escape()

          $(document).off("focusin.modal")

          this.$element
            .removeClass("in")
            .attr("aria-hidden", true)

          $.support.transition && this.$element.hasClass("fade") ?
            this.hideWithTransition() :
            this.hideModal()
      }

      , enforceFocus: function () {
          var that = this
          $(document).on("focusin.modal", function (e) {
              if (that.$element[0] !== e.target && !that.$element.has(e.target).length) {
                  that.$element.focus()
              }
          })
      }

      , escape: function () {
          var that = this
          if (this.isShown && this.options.keyboard) {
              this.$element.on("keyup.dismiss.modal", function (e) {
                  e.which == 27 && that.hide()
              })
          } else if (!this.isShown) {
              this.$element.off("keyup.dismiss.modal")
          }
      }

      , hideWithTransition: function () {
          var that = this
            , timeout = setTimeout(function () {
                that.$element.off($.support.transition.end)
                that.hideModal()
            }, 500)

          this.$element.one($.support.transition.end, function () {
              clearTimeout(timeout)
              that.hideModal()
          })
      }

      , hideModal: function () {
          var that = this
          this.$element.hide()
          this.backdrop(function () {
              that.removeBackdrop()
              that.$element.trigger("hidden")
          })
      }

      , removeBackdrop: function () {
          this.$backdrop && this.$backdrop.remove()
          this.$backdrop = null
      }

      , backdrop: function (callback) {
          var that = this
            , animate = this.$element.hasClass("fade") ? "fade" : ""

          if (this.isShown && this.options.backdrop) {
              var doAnimate = $.support.transition && animate

              this.$backdrop = $("<div class=\"modal-backdrop " + animate + "\" />")
                .appendTo(document.body)

              this.$backdrop.click(
                this.options.backdrop == "static" ?
                  $.proxy(this.$element[0].focus, this.$element[0])
                : $.proxy(this.hide, this)
              )

              if (doAnimate) this.$backdrop[0].offsetWidth // force reflow

              this.$backdrop.addClass("in")

              if (!callback) return

              doAnimate ?
                this.$backdrop.one($.support.transition.end, callback) :
                callback()

          } else if (!this.isShown && this.$backdrop) {
              this.$backdrop.removeClass("in")

              $.support.transition && this.$element.hasClass("fade") ?
                this.$backdrop.one($.support.transition.end, callback) :
                callback()

          } else if (callback) {
              callback()
          }
      }
    }


    /* MODAL PLUGIN DEFINITION
     * ======================= */

    var old = $.fn.modal

    $.fn.modal = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("modal")
              , options = $.extend({}, $.fn.modal.defaults, $this.data(), typeof option == "object" && option)
            if (!data) $this.data("modal", (data = new Modal(this, options)))
            if (typeof option == "string") data[option]()
            else if (options.show) data.show()
        })
    }

    $.fn.modal.defaults = {
        backdrop: true
      , keyboard: true
      , show: true
    }

    $.fn.modal.Constructor = Modal


    /* MODAL NO CONFLICT
     * ================= */

    $.fn.modal.noConflict = function () {
        $.fn.modal = old
        return this
    }


    /* MODAL DATA-API
     * ============== */

    $(document).on("click.modal.data-api", "[data-toggle=\"modal\"]", function (e) {
        var $this = $(this)
          , href = $this.attr("href")
          , $target = $($this.attr("data-target") || (href && href.replace(/.*(?=#[^\s]+$)/, ""))) //strip for ie7
          , option = $target.data("modal") ? "toggle" : $.extend({ remote: !/#/.test(href) && href }, $target.data(), $this.data())

        e.preventDefault()

        $target
          .modal(option)
          .one("hide", function () {
              $this.focus()
          })
    })

}(window.jQuery);

/* ============================================================
 * bootstrap-dropdown.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#dropdowns
 * ============================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ============================================================ */


!function ($) {

    "use strict"; // jshint ;_;


    /* DROPDOWN CLASS DEFINITION
     * ========================= */

    var toggle = "[data-toggle=dropdown]"
      , Dropdown = function (element) {
          var $el = $(element).on("click.dropdown.data-api", this.toggle)
          $("html").on("click.dropdown.data-api", function () {
              $el.parent().removeClass("open")
          })
      }

    Dropdown.prototype = {

        constructor: Dropdown

    , toggle: function (e) {
        var $this = $(this)
          , $parent
          , isActive

        if ($this.is(".disabled, :disabled")) return

        $parent = getParent($this)

        isActive = $parent.hasClass("open")

        clearMenus()

        if (!isActive) {
            if ("ontouchstart" in document.documentElement) {
                // if mobile we we use a backdrop because click events don't delegate
                $("<div class=\"dropdown-backdrop\"/>").insertBefore($(this)).on("click", clearMenus)
            }
            $parent.toggleClass("open")
        }

        $this.focus()

        return false
    }

    , keydown: function (e) {
        var $this
          , $items
          , $active
          , $parent
          , isActive
          , index

        if (!/(38|40|27)/.test(e.keyCode)) return

        $this = $(this)

        e.preventDefault()
        e.stopPropagation()

        if ($this.is(".disabled, :disabled")) return

        $parent = getParent($this)

        isActive = $parent.hasClass("open")

        if (!isActive || (isActive && e.keyCode == 27)) {
            if (e.which == 27) $parent.find(toggle).focus()
            return $this.click()
        }

        $items = $("[role=menu] li:not(.divider):visible a", $parent)

        if (!$items.length) return

        index = $items.index($items.filter(":focus"))

        if (e.keyCode == 38 && index > 0) index--                                        // up
        if (e.keyCode == 40 && index < $items.length - 1) index++                        // down
        if (!~index) index = 0

        $items
          .eq(index)
          .focus()
    }

    }

    function clearMenus() {
        $(".dropdown-backdrop").remove()
        $(toggle).each(function () {
            getParent($(this)).removeClass("open")
        })
    }

    function getParent($this) {
        var selector = $this.attr("data-target")
          , $parent

        if (!selector) {
            selector = $this.attr("href")
            selector = selector && /#/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, "") //strip for ie7
        }

        $parent = selector && $(selector)

        if (!$parent || !$parent.length) $parent = $this.parent()

        return $parent
    }


    /* DROPDOWN PLUGIN DEFINITION
     * ========================== */

    var old = $.fn.dropdown

    $.fn.dropdown = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("dropdown")
            if (!data) $this.data("dropdown", (data = new Dropdown(this)))
            if (typeof option == "string") data[option].call($this)
        })
    }

    $.fn.dropdown.Constructor = Dropdown


    /* DROPDOWN NO CONFLICT
     * ==================== */

    $.fn.dropdown.noConflict = function () {
        $.fn.dropdown = old
        return this
    }


    /* APPLY TO STANDARD DROPDOWN ELEMENTS
     * =================================== */

    $(document)
      .on("click.dropdown.data-api", clearMenus)
      .on("click.dropdown.data-api", ".dropdown form", function (e) { e.stopPropagation() })
      .on("click.dropdown.data-api", toggle, Dropdown.prototype.toggle)
      .on("keydown.dropdown.data-api", toggle + ", [role=menu]", Dropdown.prototype.keydown)

}(window.jQuery);

/* =============================================================
 * bootstrap-scrollspy.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#scrollspy
 * =============================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ============================================================== */


!function ($) {

    "use strict"; // jshint ;_;


    /* SCROLLSPY CLASS DEFINITION
     * ========================== */

    function ScrollSpy(element, options) {
        var process = $.proxy(this.process, this)
          , $element = $(element).is("body") ? $(window) : $(element)
          , href
        this.options = $.extend({}, $.fn.scrollspy.defaults, options)
        this.$scrollElement = $element.on("scroll.scroll-spy.data-api", process)
        this.selector = (this.options.target
          || ((href = $(element).attr("href")) && href.replace(/.*(?=#[^\s]+$)/, "")) //strip for ie7
          || "") + " .nav li > a"
        this.$body = $("body")
        this.refresh()
        this.process()
    }

    ScrollSpy.prototype = {

        constructor: ScrollSpy

      , refresh: function () {
          var self = this
            , $targets

          this.offsets = $([])
          this.targets = $([])

          $targets = this.$body
            .find(this.selector)
            .map(function () {
                var $el = $(this)
                  , href = $el.data("target") || $el.attr("href")
                  , $href = /^#\w/.test(href) && $(href)
                return ($href
                  && $href.length
                  && [[$href.position().top + (!$.isWindow(self.$scrollElement.get(0)) && self.$scrollElement.scrollTop()), href]]) || null
            })
            .sort(function (a, b) { return a[0] - b[0] })
            .each(function () {
                self.offsets.push(this[0])
                self.targets.push(this[1])
            })
      }

      , process: function () {
          var scrollTop = this.$scrollElement.scrollTop() + this.options.offset
            , scrollHeight = this.$scrollElement[0].scrollHeight || this.$body[0].scrollHeight
            , maxScroll = scrollHeight - this.$scrollElement.height()
            , offsets = this.offsets
            , targets = this.targets
            , activeTarget = this.activeTarget
            , i

          if (scrollTop >= maxScroll) {
              return activeTarget != (i = targets.last()[0])
                && this.activate(i)
          }

          for (i = offsets.length; i--;) {
              activeTarget != targets[i]
                && scrollTop >= offsets[i]
                && (!offsets[i + 1] || scrollTop <= offsets[i + 1])
                && this.activate(targets[i])
          }
      }

      , activate: function (target) {
          var active
            , selector

          this.activeTarget = target

          $(this.selector)
            .parent(".active")
            .removeClass("active")

          selector = this.selector
            + "[data-target=\"" + target + "\"],"
            + this.selector + "[href=\"" + target + "\"]"

          active = $(selector)
            .parent("li")
            .addClass("active")

          if (active.parent(".dropdown-menu").length) {
              active = active.closest("li.dropdown").addClass("active")
          }

          active.trigger("activate")
      }

    }


    /* SCROLLSPY PLUGIN DEFINITION
     * =========================== */

    var old = $.fn.scrollspy

    $.fn.scrollspy = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("scrollspy")
              , options = typeof option == "object" && option
            if (!data) $this.data("scrollspy", (data = new ScrollSpy(this, options)))
            if (typeof option == "string") data[option]()
        })
    }

    $.fn.scrollspy.Constructor = ScrollSpy

    $.fn.scrollspy.defaults = {
        offset: 10
    }


    /* SCROLLSPY NO CONFLICT
     * ===================== */

    $.fn.scrollspy.noConflict = function () {
        $.fn.scrollspy = old
        return this
    }


    /* SCROLLSPY DATA-API
     * ================== */

    $(window).on("load", function () {
        $("[data-spy=\"scroll\"]").each(function () {
            var $spy = $(this)
            $spy.scrollspy($spy.data())
        })
    })

}(window.jQuery);
/* ========================================================
 * bootstrap-tab.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#tabs
 * ========================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ======================================================== */


!function ($) {

    "use strict"; // jshint ;_;


    /* TAB CLASS DEFINITION
     * ==================== */

    var Tab = function (element) {
        this.element = $(element)
    }

    Tab.prototype = {

        constructor: Tab

    , show: function () {
        var $this = this.element
          , $ul = $this.closest("ul:not(.dropdown-menu)")
          , selector = $this.attr("data-target")
          , previous
          , $target
          , e

        if (!selector) {
            selector = $this.attr("href")
            selector = selector && selector.replace(/.*(?=#[^\s]*$)/, "") //strip for ie7
        }

        if ($this.parent("li").hasClass("active")) return

        previous = $ul.find(".active:last a")[0]

        e = $.Event("show", {
            relatedTarget: previous
        })

        $this.trigger(e)

        if (e.isDefaultPrevented()) return

        $target = $(selector)

        this.activate($this.parent("li"), $ul)
        this.activate($target, $target.parent(), function () {
            $this.trigger({
                type: "shown"
            , relatedTarget: previous
            })
        })
    }

    , activate: function (element, container, callback) {
        var $active = container.find("> .active")
          , transition = callback
              && $.support.transition
              && $active.hasClass("fade")

        function next() {
            $active
              .removeClass("active")
              .find("> .dropdown-menu > .active")
              .removeClass("active")

            element.addClass("active")

            if (transition) {
                element[0].offsetWidth // reflow for transition
                element.addClass("in")
            } else {
                element.removeClass("fade")
            }

            if (element.parent(".dropdown-menu")) {
                element.closest("li.dropdown").addClass("active")
            }

            callback && callback()
        }

        transition ?
          $active.one($.support.transition.end, next) :
          next()

        $active.removeClass("in")
    }
    }


    /* TAB PLUGIN DEFINITION
     * ===================== */

    var old = $.fn.tab

    $.fn.tab = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("tab")
            if (!data) $this.data("tab", (data = new Tab(this)))
            if (typeof option == "string") data[option]()
        })
    }

    $.fn.tab.Constructor = Tab


    /* TAB NO CONFLICT
     * =============== */

    $.fn.tab.noConflict = function () {
        $.fn.tab = old
        return this
    }


    /* TAB DATA-API
     * ============ */

    $(document).on("click.tab.data-api", "[data-toggle=\"tab\"], [data-toggle=\"pill\"]", function (e) {
        e.preventDefault()
        $(this).tab("show")
    })

}(window.jQuery);
/* ===========================================================
 * bootstrap-tooltip.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#tooltips
 * Inspired by the original jQuery.tipsy by Jason Frame
 * ===========================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================== */


!function ($) {

    "use strict"; // jshint ;_;


    /* TOOLTIP PUBLIC CLASS DEFINITION
     * =============================== */

    var Tooltip = function (element, options) {
        this.init("tooltip", element, options)
    }

    Tooltip.prototype = {

        constructor: Tooltip

    , init: function (type, element, options) {
        var eventIn
          , eventOut
          , triggers
          , trigger
          , i

        this.type = type
        this.$element = $(element)
        this.options = this.getOptions(options)
        this.enabled = true

        triggers = this.options.trigger.split(" ")

        for (i = triggers.length; i--;) {
            trigger = triggers[i]
            if (trigger == "click") {
                this.$element.on("click." + this.type, this.options.selector, $.proxy(this.toggle, this))
            } else if (trigger != "manual") {
                eventIn = trigger == "hover" ? "mouseenter" : "focus"
                eventOut = trigger == "hover" ? "mouseleave" : "blur"
                this.$element.on(eventIn + "." + this.type, this.options.selector, $.proxy(this.enter, this))
                this.$element.on(eventOut + "." + this.type, this.options.selector, $.proxy(this.leave, this))
            }
        }

        this.options.selector ?
          (this._options = $.extend({}, this.options, { trigger: "manual", selector: "" })) :
          this.fixTitle()
    }

    , getOptions: function (options) {
        options = $.extend({}, $.fn[this.type].defaults, this.$element.data(), options)

        if (options.delay && typeof options.delay == "number") {
            options.delay = {
                show: options.delay
            , hide: options.delay
            }
        }

        return options
    }

    , enter: function (e) {
        var defaults = $.fn[this.type].defaults
          , options = {}
          , self

        this._options && $.each(this._options, function (key, value) {
            if (defaults[key] != value) options[key] = value
        }, this)

        self = $(e.currentTarget)[this.type](options).data(this.type)

        if (!self.options.delay || !self.options.delay.show) return self.show()

        clearTimeout(this.timeout)
        self.hoverState = "in"
        this.timeout = setTimeout(function () {
            if (self.hoverState == "in") self.show()
        }, self.options.delay.show)
    }

    , leave: function (e) {
        var self = $(e.currentTarget)[this.type](this._options).data(this.type)

        if (this.timeout) clearTimeout(this.timeout)
        if (!self.options.delay || !self.options.delay.hide) return self.hide()

        self.hoverState = "out"
        this.timeout = setTimeout(function () {
            if (self.hoverState == "out") self.hide()
        }, self.options.delay.hide)
    }

    , show: function () {
        var $tip
          , pos
          , actualWidth
          , actualHeight
          , placement
          , tp
          , e = $.Event("show")

        if (this.hasContent() && this.enabled) {
            this.$element.trigger(e)
            if (e.isDefaultPrevented()) return
            $tip = this.tip()
            this.setContent()

            if (this.options.animation) {
                $tip.addClass("fade")
            }

            placement = typeof this.options.placement == "function" ?
              this.options.placement.call(this, $tip[0], this.$element[0]) :
              this.options.placement

            $tip
              .detach()
              .css({ top: 0, left: 0, display: "block" })

            this.options.container ? $tip.appendTo(this.options.container) : $tip.insertAfter(this.$element)

            pos = this.getPosition()

            actualWidth = $tip[0].offsetWidth
            actualHeight = $tip[0].offsetHeight

            switch (placement) {
                case "bottom":
                    tp = { top: pos.top + pos.height, left: pos.left + pos.width / 2 - actualWidth / 2 }
                    break
                case "top":
                    tp = { top: pos.top - actualHeight, left: pos.left + pos.width / 2 - actualWidth / 2 }
                    break
                case "left":
                    tp = { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left - actualWidth }
                    break
                case "right":
                    tp = { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left + pos.width }
                    break
            }

            this.applyPlacement(tp, placement)
            this.$element.trigger("shown")
        }
    }

    , applyPlacement: function (offset, placement) {
        var $tip = this.tip()
          , width = $tip[0].offsetWidth
          , height = $tip[0].offsetHeight
          , actualWidth
          , actualHeight
          , delta
          , replace

        $tip
          .offset(offset)
          .addClass(placement)
          .addClass("in")

        actualWidth = $tip[0].offsetWidth
        actualHeight = $tip[0].offsetHeight

        if (placement == "top" && actualHeight != height) {
            offset.top = offset.top + height - actualHeight
            replace = true
        }

        if (placement == "bottom" || placement == "top") {
            delta = 0

            if (offset.left < 0) {
                delta = offset.left * -2
                offset.left = 0
                $tip.offset(offset)
                actualWidth = $tip[0].offsetWidth
                actualHeight = $tip[0].offsetHeight
            }

            this.replaceArrow(delta - width + actualWidth, actualWidth, "left")
        } else {
            this.replaceArrow(actualHeight - height, actualHeight, "top")
        }

        if (replace) $tip.offset(offset)
    }

    , replaceArrow: function (delta, dimension, position) {
        this
          .arrow()
          .css(position, delta ? (50 * (1 - delta / dimension) + "%") : "")
    }

    , setContent: function () {
        var $tip = this.tip()
          , title = this.getTitle()

        $tip.find(".tooltip-inner")[this.options.html ? "html" : "text"](title)
        $tip.removeClass("fade in top bottom left right")
    }

    , hide: function () {
        var that = this
          , $tip = this.tip()
          , e = $.Event("hide")

        this.$element.trigger(e)
        if (e.isDefaultPrevented()) return

        $tip.removeClass("in")

        function removeWithAnimation() {
            var timeout = setTimeout(function () {
                $tip.off($.support.transition.end).detach()
            }, 500)

            $tip.one($.support.transition.end, function () {
                clearTimeout(timeout)
                $tip.detach()
            })
        }

        $.support.transition && this.$tip.hasClass("fade") ?
          removeWithAnimation() :
          $tip.detach()

        this.$element.trigger("hidden")

        return this
    }

    , fixTitle: function () {
        var $e = this.$element
        if ($e.attr("title") || typeof ($e.attr("data-original-title")) != "string") {
            $e.attr("data-original-title", $e.attr("title") || "").attr("title", "")
        }
    }

    , hasContent: function () {
        return this.getTitle()
    }

    , getPosition: function () {
        var el = this.$element[0]
        return $.extend({}, (typeof el.getBoundingClientRect == "function") ? el.getBoundingClientRect() : {
            width: el.offsetWidth
        , height: el.offsetHeight
        }, this.$element.offset())
    }

    , getTitle: function () {
        var title
          , $e = this.$element
          , o = this.options

        title = $e.attr("data-original-title")
          || (typeof o.title == "function" ? o.title.call($e[0]) : o.title)

        return title
    }

    , tip: function () {
        return this.$tip = this.$tip || $(this.options.template)
    }

    , arrow: function () {
        return this.$arrow = this.$arrow || this.tip().find(".tooltip-arrow")
    }

    , validate: function () {
        if (!this.$element[0].parentNode) {
            this.hide()
            this.$element = null
            this.options = null
        }
    }

    , enable: function () {
        this.enabled = true
    }

    , disable: function () {
        this.enabled = false
    }

    , toggleEnabled: function () {
        this.enabled = !this.enabled
    }

    , toggle: function (e) {
        var self = e ? $(e.currentTarget)[this.type](this._options).data(this.type) : this
        self.tip().hasClass("in") ? self.hide() : self.show()
    }

    , destroy: function () {
        this.hide().$element.off("." + this.type).removeData(this.type)
    }

    }


    /* TOOLTIP PLUGIN DEFINITION
     * ========================= */

    var old = $.fn.tooltip

    $.fn.tooltip = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("tooltip")
              , options = typeof option == "object" && option
            if (!data) $this.data("tooltip", (data = new Tooltip(this, options)))
            if (typeof option == "string") data[option]()
        })
    }

    $.fn.tooltip.Constructor = Tooltip

    $.fn.tooltip.defaults = {
        animation: true
    , placement: "top"
    , selector: false
    , template: "<div class=\"tooltip\"><div class=\"tooltip-arrow\"></div><div class=\"tooltip-inner\"></div></div>"
    , trigger: "hover focus"
    , title: ""
    , delay: 0
    , html: false
    , container: false
    }


    /* TOOLTIP NO CONFLICT
     * =================== */

    $.fn.tooltip.noConflict = function () {
        $.fn.tooltip = old
        return this
    }

}(window.jQuery);

/* ===========================================================
 * bootstrap-popover.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#popovers
 * ===========================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * =========================================================== */


!function ($) {

    "use strict"; // jshint ;_;


    /* POPOVER PUBLIC CLASS DEFINITION
     * =============================== */

    var Popover = function (element, options) {
        this.init("popover", element, options)
    }


    /* NOTE: POPOVER EXTENDS BOOTSTRAP-TOOLTIP.js
       ========================================== */

    Popover.prototype = $.extend({}, $.fn.tooltip.Constructor.prototype, {

        constructor: Popover

    , setContent: function () {
        var $tip = this.tip()
          , title = this.getTitle()
          , content = this.getContent()

        $tip.find(".popover-title")[this.options.html ? "html" : "text"](title)
        $tip.find(".popover-content")[this.options.html ? "html" : "text"](content)

        $tip.removeClass("fade top bottom left right in")
    }

    , hasContent: function () {
        return this.getTitle() || this.getContent()
    }

    , getContent: function () {
        var content
          , $e = this.$element
          , o = this.options

        content = (typeof o.content == "function" ? o.content.call($e[0]) : o.content)
          || $e.attr("data-content")

        return content
    }

    , tip: function () {
        if (!this.$tip) {
            this.$tip = $(this.options.template)
        }
        return this.$tip
    }

    , destroy: function () {
        this.hide().$element.off("." + this.type).removeData(this.type)
    }

    })


    /* POPOVER PLUGIN DEFINITION
     * ======================= */

    var old = $.fn.popover

    $.fn.popover = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("popover")
              , options = typeof option == "object" && option
            if (!data) $this.data("popover", (data = new Popover(this, options)))
            if (typeof option == "string") data[option]()
        })
    }

    $.fn.popover.Constructor = Popover

    $.fn.popover.defaults = $.extend({}, $.fn.tooltip.defaults, {
        placement: "right"
    , trigger: "click"
    , content: ""
    , template: "<div class=\"popover\"><div class=\"arrow\"></div><h3 class=\"popover-title\"></h3><div class=\"popover-content\"></div></div>"
    })


    /* POPOVER NO CONFLICT
     * =================== */

    $.fn.popover.noConflict = function () {
        $.fn.popover = old
        return this
    }

}(window.jQuery);

/* ==========================================================
 * bootstrap-affix.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#affix
 * ==========================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================== */


!function ($) {

    "use strict"; // jshint ;_;


    /* AFFIX CLASS DEFINITION
     * ====================== */

    var Affix = function (element, options) {
        this.options = $.extend({}, $.fn.affix.defaults, options)
        this.$window = $(window)
          .on("scroll.affix.data-api", $.proxy(this.checkPosition, this))
          .on("click.affix.data-api", $.proxy(function () { setTimeout($.proxy(this.checkPosition, this), 1) }, this))
        this.$element = $(element)
        this.checkPosition()
    }

    Affix.prototype.checkPosition = function () {
        if (!this.$element.is(":visible")) return

        var scrollHeight = $(document).height()
          , scrollTop = this.$window.scrollTop()
          , position = this.$element.offset()
          , offset = this.options.offset
          , offsetBottom = offset.bottom
          , offsetTop = offset.top
          , reset = "affix affix-top affix-bottom"
          , affix

        if (typeof offset != "object") offsetBottom = offsetTop = offset
        if (typeof offsetTop == "function") offsetTop = offset.top()
        if (typeof offsetBottom == "function") offsetBottom = offset.bottom()

        affix = this.unpin != null && (scrollTop + this.unpin <= position.top) ?
          false : offsetBottom != null && (position.top + this.$element.height() >= scrollHeight - offsetBottom) ?
          "bottom" : offsetTop != null && scrollTop <= offsetTop ?
          "top" : false

        if (this.affixed === affix) return

        this.affixed = affix
        this.unpin = affix == "bottom" ? position.top - scrollTop : null

        this.$element.removeClass(reset).addClass("affix" + (affix ? "-" + affix : ""))
    }


    /* AFFIX PLUGIN DEFINITION
     * ======================= */

    var old = $.fn.affix

    $.fn.affix = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("affix")
              , options = typeof option == "object" && option
            if (!data) $this.data("affix", (data = new Affix(this, options)))
            if (typeof option == "string") data[option]()
        })
    }

    $.fn.affix.Constructor = Affix

    $.fn.affix.defaults = {
        offset: 0
    }


    /* AFFIX NO CONFLICT
     * ================= */

    $.fn.affix.noConflict = function () {
        $.fn.affix = old
        return this
    }


    /* AFFIX DATA-API
     * ============== */

    $(window).on("load", function () {
        $("[data-spy=\"affix\"]").each(function () {
            var $spy = $(this)
              , data = $spy.data()

            data.offset = data.offset || {}

            data.offsetBottom && (data.offset.bottom = data.offsetBottom)
            data.offsetTop && (data.offset.top = data.offsetTop)

            $spy.affix(data)
        })
    })


}(window.jQuery);
/* ==========================================================
 * bootstrap-alert.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#alerts
 * ==========================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================== */


!function ($) {

    "use strict"; // jshint ;_;


    /* ALERT CLASS DEFINITION
     * ====================== */

    var dismiss = "[data-dismiss=\"alert\"]"
      , Alert = function (el) {
          $(el).on("click", dismiss, this.close)
      }

    Alert.prototype.close = function (e) {
        var $this = $(this)
          , selector = $this.attr("data-target")
          , $parent

        if (!selector) {
            selector = $this.attr("href")
            selector = selector && selector.replace(/.*(?=#[^\s]*$)/, "") //strip for ie7
        }

        $parent = $(selector)

        e && e.preventDefault()

        $parent.length || ($parent = $this.hasClass("alert") ? $this : $this.parent())

        $parent.trigger(e = $.Event("close"))

        if (e.isDefaultPrevented()) return

        $parent.removeClass("in")

        function removeElement() {
            $parent
              .trigger("closed")
              .remove()
        }

        $.support.transition && $parent.hasClass("fade") ?
          $parent.on($.support.transition.end, removeElement) :
          removeElement()
    }


    /* ALERT PLUGIN DEFINITION
     * ======================= */

    var old = $.fn.alert

    $.fn.alert = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("alert")
            if (!data) $this.data("alert", (data = new Alert(this)))
            if (typeof option == "string") data[option].call($this)
        })
    }

    $.fn.alert.Constructor = Alert


    /* ALERT NO CONFLICT
     * ================= */

    $.fn.alert.noConflict = function () {
        $.fn.alert = old
        return this
    }


    /* ALERT DATA-API
     * ============== */

    $(document).on("click.alert.data-api", dismiss, Alert.prototype.close)

}(window.jQuery);
/* ============================================================
 * bootstrap-button.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#buttons
 * ============================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ============================================================ */


!function ($) {

    "use strict"; // jshint ;_;


    /* BUTTON PUBLIC CLASS DEFINITION
     * ============================== */

    var Button = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, $.fn.button.defaults, options)
    }

    Button.prototype.setState = function (state) {
        var d = "disabled"
          , $el = this.$element
          , data = $el.data()
          , val = $el.is("input") ? "val" : "html"

        state = state + "Text"
        data.resetText || $el.data("resetText", $el[val]())

        $el[val](data[state] || this.options[state])

        // push to event loop to allow forms to submit
        setTimeout(function () {
            state == "loadingText" ?
              $el.addClass(d).attr(d, d) :
              $el.removeClass(d).removeAttr(d)
        }, 0)
    }

    Button.prototype.toggle = function () {
        var $parent = this.$element.closest("[data-toggle=\"buttons-radio\"]")

        $parent && $parent
          .find(".active")
          .removeClass("active")

        this.$element.toggleClass("active")
    }


    /* BUTTON PLUGIN DEFINITION
     * ======================== */

    var old = $.fn.button

    $.fn.button = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("button")
              , options = typeof option == "object" && option
            if (!data) $this.data("button", (data = new Button(this, options)))
            if (option == "toggle") data.toggle()
            else if (option) data.setState(option)
        })
    }

    $.fn.button.defaults = {
        loadingText: "loading..."
    }

    $.fn.button.Constructor = Button


    /* BUTTON NO CONFLICT
     * ================== */

    $.fn.button.noConflict = function () {
        $.fn.button = old
        return this
    }


    /* BUTTON DATA-API
     * =============== */

    $(document).on("click.button.data-api", "[data-toggle^=button]", function (e) {
        var $btn = $(e.target)
        if (!$btn.hasClass("btn")) $btn = $btn.closest(".btn")
        $btn.button("toggle")
    })

}(window.jQuery);
/* =============================================================
 * bootstrap-collapse.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#collapse
 * =============================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ============================================================ */


!function ($) {

    "use strict"; // jshint ;_;


    /* COLLAPSE PUBLIC CLASS DEFINITION
     * ================================ */

    var Collapse = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, $.fn.collapse.defaults, options)

        if (this.options.parent) {
            this.$parent = $(this.options.parent)
        }

        this.options.toggle && this.toggle()
    }

    Collapse.prototype = {

        constructor: Collapse

    , dimension: function () {
        var hasWidth = this.$element.hasClass("width")
        return hasWidth ? "width" : "height"
    }

    , show: function () {
        var dimension
          , scroll
          , actives
          , hasData

        if (this.transitioning || this.$element.hasClass("in")) return

        dimension = this.dimension()
        scroll = $.camelCase(["scroll", dimension].join("-"))
        actives = this.$parent && this.$parent.find("> .accordion-group > .in")

        if (actives && actives.length) {
            hasData = actives.data("collapse")
            if (hasData && hasData.transitioning) return
            actives.collapse("hide")
            hasData || actives.data("collapse", null)
        }

        this.$element[dimension](0)
        this.transition("addClass", $.Event("show"), "shown")
        $.support.transition && this.$element[dimension](this.$element[0][scroll])
    }

    , hide: function () {
        var dimension
        if (this.transitioning || !this.$element.hasClass("in")) return
        dimension = this.dimension()
        this.reset(this.$element[dimension]())
        this.transition("removeClass", $.Event("hide"), "hidden")
        this.$element[dimension](0)
    }

    , reset: function (size) {
        var dimension = this.dimension()

        this.$element
          .removeClass("collapse")
          [dimension](size || "auto")
          [0].offsetWidth

        this.$element[size !== null ? "addClass" : "removeClass"]("collapse")

        return this
    }

    , transition: function (method, startEvent, completeEvent) {
        var that = this
          , complete = function () {
              if (startEvent.type == "show") that.reset()
              that.transitioning = 0
              that.$element.trigger(completeEvent)
          }

        this.$element.trigger(startEvent)

        if (startEvent.isDefaultPrevented()) return

        this.transitioning = 1

        this.$element[method]("in")

        $.support.transition && this.$element.hasClass("collapse") ?
          this.$element.one($.support.transition.end, complete) :
          complete()
    }

    , toggle: function () {
        this[this.$element.hasClass("in") ? "hide" : "show"]()
    }

    }


    /* COLLAPSE PLUGIN DEFINITION
     * ========================== */

    var old = $.fn.collapse

    $.fn.collapse = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("collapse")
              , options = $.extend({}, $.fn.collapse.defaults, $this.data(), typeof option == "object" && option)
            if (!data) $this.data("collapse", (data = new Collapse(this, options)))
            if (typeof option == "string") data[option]()
        })
    }

    $.fn.collapse.defaults = {
        toggle: true
    }

    $.fn.collapse.Constructor = Collapse


    /* COLLAPSE NO CONFLICT
     * ==================== */

    $.fn.collapse.noConflict = function () {
        $.fn.collapse = old
        return this
    }


    /* COLLAPSE DATA-API
     * ================= */

    $(document).on("click.collapse.data-api", "[data-toggle=collapse]", function (e) {
        var $this = $(this), href
          , target = $this.attr("data-target")
            || e.preventDefault()
            || (href = $this.attr("href")) && href.replace(/.*(?=#[^\s]+$)/, "") //strip for ie7
          , option = $(target).data("collapse") ? "toggle" : $this.data()
        $this[$(target).hasClass("in") ? "addClass" : "removeClass"]("collapsed")
        $(target).collapse(option)
    })

}(window.jQuery);
/* ==========================================================
 * bootstrap-carousel.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#carousel
 * ==========================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================== */


!function ($) {

    "use strict"; // jshint ;_;


    /* CAROUSEL CLASS DEFINITION
     * ========================= */

    var Carousel = function (element, options) {
        this.$element = $(element)
        this.$indicators = this.$element.find(".carousel-indicators")
        this.options = options
        this.options.pause == "hover" && this.$element
          .on("mouseenter", $.proxy(this.pause, this))
          .on("mouseleave", $.proxy(this.cycle, this))
    }

    Carousel.prototype = {

        cycle: function (e) {
            if (!e) this.paused = false
            if (this.interval) clearInterval(this.interval);
            this.options.interval
              && !this.paused
              && (this.interval = setInterval($.proxy(this.next, this), this.options.interval))
            return this
        }

    , getActiveIndex: function () {
        this.$active = this.$element.find(".item.active")
        this.$items = this.$active.parent().children()
        return this.$items.index(this.$active)
    }

    , to: function (pos) {
        var activeIndex = this.getActiveIndex()
          , that = this

        if (pos > (this.$items.length - 1) || pos < 0) return

        if (this.sliding) {
            return this.$element.one("slid", function () {
                that.to(pos)
            })
        }

        if (activeIndex == pos) {
            return this.pause().cycle()
        }

        return this.slide(pos > activeIndex ? "next" : "prev", $(this.$items[pos]))
    }

    , pause: function (e) {
        if (!e) this.paused = true
        if (this.$element.find(".next, .prev").length && $.support.transition.end) {
            this.$element.trigger($.support.transition.end)
            this.cycle(true)
        }
        clearInterval(this.interval)
        this.interval = null
        return this
    }

    , next: function () {
        if (this.sliding) return
        return this.slide("next")
    }

    , prev: function () {
        if (this.sliding) return
        return this.slide("prev")
    }

    , slide: function (type, next) {
        var $active = this.$element.find(".item.active")
          , $next = next || $active[type]()
          , isCycling = this.interval
          , direction = type == "next" ? "left" : "right"
          , fallback = type == "next" ? "first" : "last"
          , that = this
          , e

        this.sliding = true

        isCycling && this.pause()

        $next = $next.length ? $next : this.$element.find(".item")[fallback]()

        e = $.Event("slide", {
            relatedTarget: $next[0]
        , direction: direction
        })

        if ($next.hasClass("active")) return

        if (this.$indicators.length) {
            this.$indicators.find(".active").removeClass("active")
            this.$element.one("slid", function () {
                var $nextIndicator = $(that.$indicators.children()[that.getActiveIndex()])
                $nextIndicator && $nextIndicator.addClass("active")
            })
        }

        if ($.support.transition && this.$element.hasClass("slide")) {
            this.$element.trigger(e)
            if (e.isDefaultPrevented()) return
            $next.addClass(type)
            $next[0].offsetWidth // force reflow
            $active.addClass(direction)
            $next.addClass(direction)
            this.$element.one($.support.transition.end, function () {
                $next.removeClass([type, direction].join(" ")).addClass("active")
                $active.removeClass(["active", direction].join(" "))
                that.sliding = false
                setTimeout(function () { that.$element.trigger("slid") }, 0)
            })
        } else {
            this.$element.trigger(e)
            if (e.isDefaultPrevented()) return
            $active.removeClass("active")
            $next.addClass("active")
            this.sliding = false
            this.$element.trigger("slid")
        }

        isCycling && this.cycle()

        return this
    }

    }


    /* CAROUSEL PLUGIN DEFINITION
     * ========================== */

    var old = $.fn.carousel

    $.fn.carousel = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("carousel")
              , options = $.extend({}, $.fn.carousel.defaults, typeof option == "object" && option)
              , action = typeof option == "string" ? option : options.slide
            if (!data) $this.data("carousel", (data = new Carousel(this, options)))
            if (typeof option == "number") data.to(option)
            else if (action) data[action]()
            else if (options.interval) data.pause().cycle()
        })
    }

    $.fn.carousel.defaults = {
        interval: 5000
    , pause: "hover"
    }

    $.fn.carousel.Constructor = Carousel


    /* CAROUSEL NO CONFLICT
     * ==================== */

    $.fn.carousel.noConflict = function () {
        $.fn.carousel = old
        return this
    }

    /* CAROUSEL DATA-API
     * ================= */

    $(document).on("click.carousel.data-api", "[data-slide], [data-slide-to]", function (e) {
        var $this = $(this), href
          , $target = $($this.attr("data-target") || (href = $this.attr("href")) && href.replace(/.*(?=#[^\s]+$)/, "")) //strip for ie7
          , options = $.extend({}, $target.data(), $this.data())
          , slideIndex

        $target.carousel(options)

        if (slideIndex = $this.attr("data-slide-to")) {
            $target.data("carousel").pause().to(slideIndex).cycle()
        }

        e.preventDefault()
    })

}(window.jQuery);
/* =============================================================
 * bootstrap-typeahead.js v2.3.2
 * http://twitter.github.com/bootstrap/javascript.html#typeahead
 * =============================================================
 * Copyright 2012 Twitter, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ============================================================ */


!function ($) {

    "use strict"; // jshint ;_;


    /* TYPEAHEAD PUBLIC CLASS DEFINITION
     * ================================= */

    var Typeahead = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, $.fn.typeahead.defaults, options)
        this.matcher = this.options.matcher || this.matcher
        this.sorter = this.options.sorter || this.sorter
        this.highlighter = this.options.highlighter || this.highlighter
        this.updater = this.options.updater || this.updater
        this.source = this.options.source
        this.$menu = $(this.options.menu)
        this.shown = false
        this.listen()
    }

    Typeahead.prototype = {

        constructor: Typeahead

    , select: function () {
        var val = this.$menu.find(".active").attr("data-value")
        this.$element
          .val(this.updater(val))
          .change()
        return this.hide()
    }

    , updater: function (item) {
        return item
    }

    , show: function () {
        var pos = $.extend({}, this.$element.position(), {
            height: this.$element[0].offsetHeight
        })

        this.$menu
          .insertAfter(this.$element)
          .css({
              top: pos.top + pos.height
          , left: pos.left
          })
          .show()

        this.shown = true
        return this
    }

    , hide: function () {
        this.$menu.hide()
        this.shown = false
        return this
    }

    , lookup: function (event) {
        var items

        this.query = this.$element.val()

        if (!this.query || this.query.length < this.options.minLength) {
            return this.shown ? this.hide() : this
        }

        items = isFunction(this.source) ? this.source(this.query, $.proxy(this.process, this)) : this.source

        return items ? this.process(items) : this
    }

    , process: function (items) {
        var that = this

        items = $.grep(items, function (item) {
            return that.matcher(item)
        })

        items = this.sorter(items)

        if (!items.length) {
            return this.shown ? this.hide() : this
        }

        return this.render(items.slice(0, this.options.items)).show()
    }

    , matcher: function (item) {
        return ~item.toLowerCase().indexOf(this.query.toLowerCase())
    }

    , sorter: function (items) {
        var beginswith = []
          , caseSensitive = []
          , caseInsensitive = []
          , item

        while (item = items.shift()) {
            if (!item.toLowerCase().indexOf(this.query.toLowerCase())) beginswith.push(item)
            else if (~item.indexOf(this.query)) caseSensitive.push(item)
            else caseInsensitive.push(item)
        }

        return beginswith.concat(caseSensitive, caseInsensitive)
    }

    , highlighter: function (item) {
        var query = this.query.replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, "\\$&")
        return item.replace(new RegExp("(" + query + ")", "ig"), function ($1, match) {
            return "<strong>" + match + "</strong>"
        })
    }

    , render: function (items) {
        var that = this

        items = $(items).map(function (i, item) {
            i = $(that.options.item).attr("data-value", item)
            i.find("a").html(that.highlighter(item))
            return i[0]
        })

        items.first().addClass("active")
        this.$menu.html(items)
        return this
    }

    , next: function (event) {
        var active = this.$menu.find(".active").removeClass("active")
          , next = active.next()

        if (!next.length) {
            next = $(this.$menu.find("li")[0])
        }

        next.addClass("active")
    }

    , prev: function (event) {
        var active = this.$menu.find(".active").removeClass("active")
          , prev = active.prev()

        if (!prev.length) {
            prev = this.$menu.find("li").last()
        }

        prev.addClass("active")
    }

    , listen: function () {
        this.$element
          .on("focus", $.proxy(this.focus, this))
          .on("blur", $.proxy(this.blur, this))
          .on("keypress", $.proxy(this.keypress, this))
          .on("keyup", $.proxy(this.keyup, this))

        if (this.eventSupported("keydown")) {
            this.$element.on("keydown", $.proxy(this.keydown, this))
        }

        this.$menu
          .on("click", $.proxy(this.click, this))
          .on("mouseenter", "li", $.proxy(this.mouseenter, this))
          .on("mouseleave", "li", $.proxy(this.mouseleave, this))
    }

    , eventSupported: function (eventName) {
        var isSupported = eventName in this.$element
        if (!isSupported) {
            this.$element.setAttribute(eventName, "return;")
            isSupported = typeof this.$element[eventName] === "function"
        }
        return isSupported
    }

    , move: function (e) {
        if (!this.shown) return

        switch (e.keyCode) {
            case 9: // tab
            case 13: // enter
            case 27: // escape
                e.preventDefault()
                break

            case 38: // up arrow
                e.preventDefault()
                this.prev()
                break

            case 40: // down arrow
                e.preventDefault()
                this.next()
                break
        }

        e.stopPropagation()
    }

    , keydown: function (e) {
        this.suppressKeyPressRepeat = ~$.inArray(e.keyCode, [40, 38, 9, 13, 27])
        this.move(e)
    }

    , keypress: function (e) {
        if (this.suppressKeyPressRepeat) return
        this.move(e)
    }

    , keyup: function (e) {
        switch (e.keyCode) {
            case 40: // down arrow
            case 38: // up arrow
            case 16: // shift
            case 17: // ctrl
            case 18: // alt
                break

            case 9: // tab
            case 13: // enter
                if (!this.shown) return
                this.select()
                break

            case 27: // escape
                if (!this.shown) return
                this.hide()
                break

            default:
                this.lookup()
        }

        e.stopPropagation()
        e.preventDefault()
    }

    , focus: function (e) {
        this.focused = true
    }

    , blur: function (e) {
        this.focused = false
        if (!this.mousedover && this.shown) this.hide()
    }

    , click: function (e) {
        e.stopPropagation()
        e.preventDefault()
        this.select()
        this.$element.focus()
    }

    , mouseenter: function (e) {
        this.mousedover = true
        this.$menu.find(".active").removeClass("active")
        $(e.currentTarget).addClass("active")
    }

    , mouseleave: function (e) {
        this.mousedover = false
        if (!this.focused && this.shown) this.hide()
    }

    }


    /* TYPEAHEAD PLUGIN DEFINITION
     * =========================== */

    var old = $.fn.typeahead

    $.fn.typeahead = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data("typeahead")
              , options = typeof option == "object" && option
            if (!data) $this.data("typeahead", (data = new Typeahead(this, options)))
            if (typeof option == "string") data[option]()
        })
    }

    $.fn.typeahead.defaults = {
        source: []
    , items: 8
    , menu: "<ul class=\"typeahead dropdown-menu\"></ul>"
    , item: "<li><a href=\"#\"></a></li>"
    , minLength: 1
    }

    $.fn.typeahead.Constructor = Typeahead


    /* TYPEAHEAD NO CONFLICT
     * =================== */

    $.fn.typeahead.noConflict = function () {
        $.fn.typeahead = old
        return this
    }


    /* TYPEAHEAD DATA-API
     * ================== */

    $(document).on("focus.typeahead.data-api", "[data-provide=\"typeahead\"]", function (e) {
        var $this = $(this)
        if ($this.data("typeahead")) return
        $this.typeahead($this.data())
    })

}(window.jQuery);