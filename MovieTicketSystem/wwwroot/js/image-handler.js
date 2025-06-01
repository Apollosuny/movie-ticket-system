function handleImageError(img) {
  console.log('Image failed to load:', img.src);
  img.onerror = null; // Prevent endless loop if fallback also fails
  var movieTitle = img.alt || 'Movie';
  img.src =
    'https://via.placeholder.com/400x600?text=' +
    encodeURIComponent(movieTitle);
}

document.addEventListener('DOMContentLoaded', function () {
  // Find all movie poster images and add error handling
  var movieImages = document.querySelectorAll('img[src^="/images/movies/"]');
  console.log('Found ' + movieImages.length + ' movie images to monitor');

  movieImages.forEach(function (img) {
    img.addEventListener('error', function () {
      handleImageError(this);
    });
  });
});
