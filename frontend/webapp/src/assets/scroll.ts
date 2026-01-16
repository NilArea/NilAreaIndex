import $ from 'jquery';

const $body = $('html, body');
export function navScroll(trigger: JQuery) {
  trigger.on('click', function(e) {
    e.preventDefault();
    const targetId = $(this).data('target');
    if (targetId[0] === '#') {
      const scrollPosition = parseInt(targetId.substring(1), 10);
      if (isNaN(scrollPosition))
        return;
      else {
        $body.animate({
          scrollTop: scrollPosition
        }, 1000, 'swing');
      }
    }
    const target = $('#' + targetId);
    const offset = target.offset();
    if (offset === undefined) {
      return;
    }
    if (target.length) {
      $body.animate({
        scrollTop: offset.top
      }, 1000, 'swing');
    }
  });
}
