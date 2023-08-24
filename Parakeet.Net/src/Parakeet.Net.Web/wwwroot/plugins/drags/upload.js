$(function(){ 
    //阻止浏览器默认行。 
    $(document).on({ 
        dragleave:function(e){    //拖离 
            e.preventDefault(); 
        }, 
        drop:function(e){  //拖后放 
            e.preventDefault(); 
        }, 
        dragenter:function(e){    //拖进 
            e.preventDefault(); 
        }, 
        dragover:function(e){    //拖来拖去 
            e.preventDefault(); 
        } 
    }); 
	
    var box = document.getElementById('dropbox'); //拖拽区域 
    box.addEventListener('drop',function(e){ 
        e.preventDefault(); //取消默认浏览器拖拽效果 
        var fileList = e.dataTransfer.files; //获取文件对象 
        debugger;
        //这里面可以获取文件信息，然后缓存下文件src等，给dxGallery/dxTileView控件的数据源展示出来
        //控制控件删除功能
		if(!fileList.length){ 
			return false; 
		}
		
		//处理多文件上传
		var _i=0,_filenum=fileList.length;
		
		//定义上传文件函数
		var uploadFiles = function(){
			var file=fileList[_i];
			
			//判断类型
			if(file.type.indexOf('image') === -1){ 
				alert('您拖的不是图片！'); 
				return false; 
			} 
			
			//判断大小
			var filesize = Math.floor((file.size)/1024);  
			if(filesize>2000){ 
				alert('上传大小不能超过2M.'); 
				return false; 
			} 

			//上传进度
			document.getElementById('dropbox').innerHTML='上传中...('+_i+'/'+_filenum+')';
			 
			//上传 
			xhr = new XMLHttpRequest(); 
			
			//进度条
			if(xhr.upload){
				xhr.upload.onprogress=function(e){
					var pert=e.loaded/e.total*99+'%';
					document.getElementById('upload_progress').style.width=pert;
				};
			}
			
			// 文件上传成功或是失败
			xhr.onreadystatechange=function(e){
				if(this.readyState==4){
					//这里可以处理一下上传结果！
					//服务器端的PHP对于错误可以返回一个Error，成功返回图片地址
					//if(this.responseText.indexOf('Error') === -1){
					//	var imageUrl=this.responseText;
					//	alert(imageUrl);
					//}else{
					//	alert('文件上传失败OwQ');
					//}
					
					//上传下一个文件
					_i++;
					if(_i<_filenum){
						uploadFiles();
					}else{
						document.getElementById('dropbox').innerHTML='上传完成';
					}
					//清零上传条
					document.getElementById('upload_progress').style.width='0%';
				}
			};
			
			//这里处理上传文件的服务器地址
			//跨域的话注意一下
			xhr.open('post', 'upload.php', true); 
			xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest'); 
			var formdata = new FormData(); 
			formdata.append('file', file); 
			xhr.send(formdata); 
		};
		
		//开始上传文件
		//uploadFiles();
		
    },false); 
}); 
