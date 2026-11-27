// Gestión de tareas en el detalle de un proyecto (HU-05).
// Los formularios se envían con fetch al controlador MVC TareasController, que llama a la API y
// devuelve HTML: la sección de tareas actualizada (se reemplaza sin recargar la página, CA-05.1 y
// CA-05.2) o el formulario con los errores de validación (400). El JavaScript nunca llama a la API.
(() => {
    const contenedor = document.getElementById('tareas-proyecto');
    const modalElemento = document.getElementById('modalTarea');
    if (!contenedor || !modalElemento) {
        return;
    }

    const contenidoModal = document.getElementById('modalTareaContenido');
    const modal = bootstrap.Modal.getOrCreateInstance(modalElemento);

    // El encabezado X-Requested-With hace que la aplicación responda 401/403 en lugar de redirigir.
    async function solicitar(url, opciones = {}) {
        const respuesta = await fetch(url, {
            ...opciones,
            headers: { 'X-Requested-With': 'fetch' },
            credentials: 'same-origin'
        });

        if (respuesta.status === 401) {
            // La sesión venció: al recargar, la aplicación lleva al login y luego de vuelta aquí.
            window.location.reload();
            return null;
        }

        return respuesta;
    }

    function mensajeDeError(estado) {
        switch (estado) {
            case 403: return 'Su rol no permite esta operación.';
            case 404: return 'La tarea o el proyecto ya no existe.';
            default: return 'No se pudo contactar el servicio. Intente de nuevo en unos minutos.';
        }
    }

    function mostrarFormulario(html) {
        contenidoModal.innerHTML = html;
        const formulario = contenidoModal.querySelector('form');
        // La validación del navegador se enlaza a formularios cargados después de abrir la página.
        if (formulario && window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
            jQuery.validator.unobtrusive.parse(formulario);
        }
    }

    function mostrarError(mensaje) {
        contenidoModal.innerHTML =
            '<div class="modal-content">' +
            '  <div class="modal-body"><div class="alert alert-danger mb-0" role="alert"></div></div>' +
            '  <div class="modal-footer">' +
            '    <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cerrar</button>' +
            '  </div>' +
            '</div>';
        contenidoModal.querySelector('.alert').textContent = mensaje;
        modal.show();
    }

    // Abrir el formulario de nueva tarea o de edición.
    document.addEventListener('click', async (evento) => {
        const boton = evento.target.closest('[data-tarea-formulario]');
        if (!boton) {
            return;
        }

        boton.disabled = true;
        try {
            const respuesta = await solicitar(boton.dataset.tareaFormulario);
            if (!respuesta) {
                return;
            }

            if (!respuesta.ok) {
                mostrarError(mensajeDeError(respuesta.status));
                return;
            }

            mostrarFormulario(await respuesta.text());
            modal.show();
        } catch {
            mostrarError(mensajeDeError(0));
        } finally {
            boton.disabled = false;
        }
    });

    // Guardar una tarea o cancelarla.
    document.addEventListener('submit', async (evento) => {
        const formulario = evento.target;
        const esFormularioTarea = formulario.id === 'formTarea';
        const confirmacionCancelar = formulario.dataset.tareaCancelar;
        if (!esFormularioTarea && confirmacionCancelar === undefined) {
            return;
        }

        evento.preventDefault();

        if (esFormularioTarea && window.jQuery && jQuery.fn.valid && !jQuery(formulario).valid()) {
            return;
        }

        if (confirmacionCancelar && !window.confirm(confirmacionCancelar)) {
            return;
        }

        const boton = formulario.querySelector('[type="submit"]');
        if (boton) {
            boton.disabled = true;
        }

        try {
            const respuesta = await solicitar(formulario.action, { method: 'POST', body: new FormData(formulario) });
            if (!respuesta) {
                return;
            }

            if (respuesta.ok) {
                contenedor.innerHTML = await respuesta.text();
                modal.hide();
                return;
            }

            if (respuesta.status === 400 && esFormularioTarea) {
                mostrarFormulario(await respuesta.text());
                return;
            }

            mostrarError(mensajeDeError(respuesta.status));
        } catch {
            mostrarError(mensajeDeError(0));
        } finally {
            if (boton && boton.isConnected) {
                boton.disabled = false;
            }
        }
    });
})();
