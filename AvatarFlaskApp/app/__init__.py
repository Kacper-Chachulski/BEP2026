import mimetypes
from flask import request

# Ensure `.wasm` is served with the correct MIME type
mimetypes.add_type('application/wasm', '.wasm')
# Unity sometimes renames compressed files to .unityweb — map common extensions
mimetypes.add_type('application/wasm', '.wasm.unityweb')
mimetypes.add_type('application/javascript', '.js')
mimetypes.add_type('application/javascript', '.js.unityweb')
mimetypes.add_type('application/octet-stream', '.data')
mimetypes.add_type('application/octet-stream', '.data.unityweb')


def init_unity_response_headers(app):
	"""Register an after_request handler to set Content-Type and Content-Encoding
	for Unity WebGL files served under the `/AvatarGame` static route.

	Add a call to `init_unity_response_headers(app)` from your app factory
	or import this module early so the types/handler are registered.
	"""

	@app.after_request
	def _set_unity_headers(response):
		path = request.path or ''
		if path.startswith('/AvatarGame/'):
			# If the build files were pre-gzipped (this project stores the
			# compressed files as plain names, e.g. `AvatarGame.wasm`), set
			# `Content-Encoding: gzip` for those specific files so the browser
			# will decompress them before WebAssembly instantiation.
			if path.endswith('/AvatarGame.wasm') or path.endswith('/AvatarGame.data') or path.endswith('/AvatarGame.framework.js'):
				response.headers['Content-Encoding'] = 'gzip'

			# Ensure .wasm content-type is correct
			if path.endswith('.wasm') or path.endswith('.wasm.unityweb'):
				response.headers['Content-Type'] = 'application/wasm'

			# Data and loader files
			if path.endswith('.data') or path.endswith('.data.unityweb'):
				response.headers['Content-Type'] = 'application/octet-stream'
			if path.endswith('.js') or path.endswith('.js.unityweb') or path.endswith('.framework.js'):
				response.headers['Content-Type'] = 'application/javascript'

		return response


__all__ = ['init_unity_response_headers']
