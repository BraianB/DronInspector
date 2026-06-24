CREATE TABLE tb_master_control
(
    id SERIAL PRIMARY KEY,
    fecha_ejecucion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    tamanio_terreno INT NOT NULL,
    coordenada_x INT NOT NULL,
    coordenada_y INT NOT NULL
);

CREATE TABLE tb_det_log
(
    id SERIAL PRIMARY KEY,
    master_id INT NOT NULL,
    paso INT NOT NULL,
    coordenada_x INT NOT NULL,
    coordenada_y INT NOT NULL,

    CONSTRAINT fk_master
        FOREIGN KEY(master_id)
        REFERENCES tb_master_control(id)
);