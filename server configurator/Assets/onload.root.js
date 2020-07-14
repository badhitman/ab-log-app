jQuery(document).ready(function () {
    console.trace('root onload document (ready)');
    jQuery('input[type=submit]').click(function ()
    {
        jQuery(this).prop("disabled", true)
    });
});