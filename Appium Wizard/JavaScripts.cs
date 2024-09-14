using System.Text;

namespace Appium_Wizard
{
    public static class JavaScripts
    {
        public static string DrawRectangleOnCanvas()
        {
            var sb = new StringBuilder(@"
    function drawRectangle(x, y, width, height, line, color, clear) {
        var canvas = document.getElementById('overlayCanvas');
        if (canvas === null) canvas = createOverlayCanvas({clickThrough: true});
        const ctx = canvas.getContext('2d');

        if (clear) {
            ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        }

        ctx.beginPath();
        ctx.lineWidth = line;
        ctx.strokeStyle = color;
        ctx.rect(x, y, width, height);
        ctx.stroke();
        
        setTimeout(function() {
            ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        }, 1000); // 1 second delay
    }

    function drawDot(x, y, radius, color, clear) {
        var canvas = document.getElementById('overlayCanvas');
        if (canvas === null) canvas = createOverlayCanvas({clickThrough: true});
        const ctx = canvas.getContext('2d');

        if (clear) {
            ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        }

        ctx.beginPath();
        ctx.arc(x, y, radius, 0, 2 * Math.PI);
        ctx.fillStyle = color;
        ctx.fill();

        setTimeout(function() {
            ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        }, 1000); // 1 second delay
    }

    function drawArrow(startX, startY, endX, endY, line, color, clear) {
        var canvas = document.getElementById('overlayCanvas');
        if (canvas === null) canvas = createOverlayCanvas({clickThrough: true});
        const ctx = canvas.getContext('2d');

        if (clear) {
            ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        }

        const headLength = 10; // Length of the arrow head
        const dx = endX - startX;
        const dy = endY - startY;
        const angle = Math.atan2(dy, dx);

        ctx.beginPath();
        ctx.lineWidth = line;
        ctx.strokeStyle = color;
        ctx.moveTo(startX, startY);
        ctx.lineTo(endX, endY);
        ctx.lineTo(endX - headLength * Math.cos(angle - Math.PI / 6), endY - headLength * Math.sin(angle - Math.PI / 6));
        ctx.moveTo(endX, endY);
        ctx.lineTo(endX - headLength * Math.cos(angle + Math.PI / 6), endY - headLength * Math.sin(angle + Math.PI / 6));
        ctx.stroke();
        ctx.closePath();

        setTimeout(function() {
            ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        }, 1000); // 1 second delay
    }

    function createOverlayCanvas(clickThrough) {
        let overlay = document.createElement('canvas');
        overlay.id = 'overlayCanvas';
        overlay.style.top = 0;
        overlay.style.left = 0;

        // Adjust the canvas size to fit within the available space in the form
        const formWidth = document.body.clientWidth;
        const formHeight = document.body.clientHeight;
        overlay.width = formWidth * window.devicePixelRatio;
        overlay.height = formHeight * window.devicePixelRatio;
        overlay.style.width = formWidth + 'px';
        overlay.style.height = formHeight + 'px';

        overlay.style.position = 'absolute';
        overlay.style.overflow = 'hidden';

        if (clickThrough) overlay.style.pointerEvents = 'none';

        document.body.appendChild(overlay);
        overlay.style.zIndex = '1';

        return overlay;
    }
    ");
            return sb.ToString();
        }
    }
}