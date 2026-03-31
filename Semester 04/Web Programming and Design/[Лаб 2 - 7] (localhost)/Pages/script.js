document.getElementById('image-upload-button').addEventListener('click', function() {
    document.getElementById('image-upload').click();
  });
  
  document.getElementById('image-upload').addEventListener('change', function(e) {
    var file = e.target.files[0];
    if (file) {

      console.log('Выбран файл:', file);
    }
  });
  